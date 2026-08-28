using System;
using System.Collections.Generic;

namespace WxSharp;

/// <summary>Anything event handlers can be bound to, following <c>wxEvtHandler</c>.</summary>
///
/// <remarks>
/// wxWidgets puts event handling below windows rather than on them: <c>wxWindow</c> and <c>wxApp</c> are
/// both <c>wxEvtHandler</c>s, which is why a timer can be owned by either and why an application can answer
/// events of its own. wxPython models the same hierarchy, and so does this.
///
/// There is one event path. The typed <c>event</c> members on <see cref="Window"/> and on every control are
/// shorthand for <see cref="Bind{TEventArgs}"/>, and both end up in the same per-event subscriber list. An
/// event type is hooked natively the first time something subscribes to it here and unhooked when the last
/// subscriber goes away, so an event nothing is listening for never crosses the boundary.
///
/// Handling and propagation are wxWidgets'. An event is handled - and so stops - unless a handler calls
/// <see cref="WxEventArgs.Skip"/>; a skipped command event then travels up the real parent chain, so binding
/// <see cref="WxEvents.ButtonClicked"/> on a frame catches its buttons, exactly as in Phoenix. The wrapper
/// does not re-dispatch events to parents itself, and treats every event the same way.
/// </remarks>
public abstract class EvtHandler
{
    private readonly Dictionary<int, List<Subscription>> _subscriptions = new();
    private readonly Dictionary<int, EventArgsFactory> _factories = new();
    private long _nextBindingToken;

    /// <summary>The identity the native side reports events against.</summary>
    internal long Token { get; private protected set; }

    /// <summary>The application this belongs to.</summary>
    internal abstract App OwnerApp { get; }

    // ---- What a concrete handler has to supply ----------------------------------------------------------

    /// <summary>Hooks one event natively. Returns false when this object cannot report it - a text-entry
    /// event on a control created without <c>TextCtrlStyle.ProcessEnter</c>, for instance.</summary>
    private protected abstract bool BindNative(int eventId);

    /// <summary>Releases the native hook for one event.</summary>
    private protected abstract void UnbindNative(int eventId);

    /// <summary>Whether the underlying object has gone, so subscriptions should be dropped rather than
    /// acted on.</summary>
    private protected abstract bool IsDead { get; }

    /// <summary>Throws unless this object is alive and being used from the UI thread.</summary>
    private protected abstract void Verify();

    // ---- Binding ----------------------------------------------------------------------------------------

    /// <summary>Subscribes to <paramref name="eventType"/>, optionally filtered to one command ID or an
    /// inclusive ID range. Dispose the returned binding to unsubscribe.</summary>
    public EventBinding Bind<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs> handler,
        int id = WindowId.Any, int lastId = WindowId.Any) where TEventArgs : WxEventArgs
    {
        ArgumentNullException.ThrowIfNull(eventType);
        ArgumentNullException.ThrowIfNull(handler);
        Verify();
        if (lastId != WindowId.Any && id == WindowId.Any)
            throw new ArgumentException("An ID range requires a starting ID.", nameof(id));
        if (lastId != WindowId.Any && lastId < id)
            throw new ArgumentOutOfRangeException(nameof(lastId));

        var token = ++_nextBindingToken;
        Subscribe(eventType.EventId, eventType.Factory,
            new Subscription(token, id, lastId, handler, args => handler(args.Source, (TEventArgs)args)));
        return new EventBinding(this, eventType.EventId, token);
    }

    /// <summary>Removes a subscription added by <see cref="Bind{TEventArgs}"/> with the same event type,
    /// handler and ID filter. Returns false when no such subscription exists.</summary>
    public bool Unbind<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs>? handler = null,
        int id = WindowId.Any, int lastId = WindowId.Any) where TEventArgs : WxEventArgs
    {
        ArgumentNullException.ThrowIfNull(eventType);
        Verify();
        if (!_subscriptions.TryGetValue(eventType.EventId, out var list)) return false;
        var index = list.FindIndex(entry => entry.Id == id && entry.LastId == lastId &&
            (handler is null || entry.Original.Equals(handler)));
        if (index < 0) return false;
        list.RemoveAt(index);
        ReleaseIfUnused(eventType.EventId, list);
        return true;
    }

    /// <summary>Backs a typed <c>event</c> accessor. Subscriptions added this way are removed by handler
    /// identity, which is what <c>-=</c> gives us.</summary>
    private protected void AddHandler<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs> handler)
        where TEventArgs : WxEventArgs
    {
        ArgumentNullException.ThrowIfNull(handler);
        Verify();
        Subscribe(eventType.EventId, eventType.Factory,
            new Subscription(++_nextBindingToken, WindowId.Any, WindowId.Any, handler,
                args => handler(args.Source, (TEventArgs)args)));
    }

    private protected void RemoveHandler<TEventArgs>(EventType<TEventArgs> eventType, EventHandler<TEventArgs> handler)
        where TEventArgs : WxEventArgs
    {
        if (IsDead || handler is null) return;
        if (!_subscriptions.TryGetValue(eventType.EventId, out var list)) return;
        var index = list.FindIndex(entry => entry.Original.Equals(handler));
        if (index < 0) return;
        list.RemoveAt(index);
        ReleaseIfUnused(eventType.EventId, list);
    }

    private void Subscribe(int eventId, EventArgsFactory factory, Subscription subscription)
    {
        if (!_subscriptions.TryGetValue(eventId, out var list))
        {
            list = new List<Subscription>();
            _subscriptions[eventId] = list;
            _factories[eventId] = factory;
            // The first subscriber is what hooks the event natively. A few events are reported whether or
            // not anyone asked, so they need no hook.
            if (!EventId.IsAlwaysReported(eventId) && !BindNative(eventId))
            {
                _subscriptions.Remove(eventId);
                _factories.Remove(eventId);
                throw new NotSupportedException(
                    $"This object cannot report event {eventId}. Text-entry events, for example, require a " +
                    "control created with TextCtrlStyle.ProcessEnter.");
            }
        }
        list.Add(subscription);
    }

    private void ReleaseIfUnused(int eventId, List<Subscription> list)
    {
        if (list.Count != 0) return;
        _subscriptions.Remove(eventId);
        _factories.Remove(eventId);
        if (!EventId.IsAlwaysReported(eventId) && !IsDead)
            UnbindNative(eventId);
    }

    internal void RemoveBinding(int eventId, long token)
    {
        if (IsDead) return;
        OwnerApp.VerifyAccess();
        if (!_subscriptions.TryGetValue(eventId, out var list)) return;
        list.RemoveAll(entry => entry.Token == token);
        ReleaseIfUnused(eventId, list);
    }

    /// <summary>Drops every subscription without touching the native side, for when the underlying object
    /// has already gone.</summary>
    private protected void ClearSubscriptions()
    {
        _subscriptions.Clear();
        _factories.Clear();
    }

    // ---- Dispatch ---------------------------------------------------------------------------------------

    /// <summary>Delivers one native event to this object's subscribers. Returns the ABI result flags:
    /// bit 0 asks wxWidgets to skip the event, bit 1 vetoes it.</summary>
    internal uint Dispatch(in NativeEvent e)
    {
        // Nothing listening is the same as every handler skipping.
        if (!_subscriptions.TryGetValue(e.Kind, out var list) || list.Count == 0) return SkipResult;

        var args = _factories[e.Kind](this, in e);
        var skipped = true;
        // Copied so a handler may unsubscribe, or subscribe, while the event is being delivered.
        foreach (var subscription in list.ToArray())
        {
            if (!subscription.Matches(e.Id)) continue;
            // Each handler decides for itself, exactly as separate wxWidgets bindings would: the next one
            // runs only if this one skipped.
            args.ResetSkipped();
            subscription.Invoke(args);
            skipped = args.Skipped;
            if (!skipped) break;
        }
        return Result(args, skipped);
    }

    private protected const uint SkipResult = 1;
    private protected const uint VetoResult = 2;

    /// <summary>Delivers a synthesised event to this object's own subscribers without involving wxWidgets.
    /// For controls that must announce a change the native control stays silent about - a programmatic value
    /// change a screen reader still needs to hear, for instance.</summary>
    private protected uint RaiseLocal(in NativeEvent e) => Dispatch(in e);

    private static uint Result(WxEventArgs args, bool skipped)
    {
        var result = skipped ? SkipResult : 0u;
        if (args is NotifyEventArgs { IsAllowed: false } ||
            args is CloseEventArgs { Vetoed: true, CanVeto: true })
            result |= VetoResult;
        return result;
    }

    private sealed record Subscription(long Token, int Id, int LastId, Delegate Original, Action<WxEventArgs> Invoke)
    {
        // wxWidgets filters by command ID at bind time; the wrapper binds once per event type and filters
        // here, which gives the same result for both the single-ID and the inclusive-range forms.
        internal bool Matches(int eventId) => Id == WindowId.Any || eventId == Id ||
            (LastId != WindowId.Any && eventId >= Id && eventId <= LastId);
    }
}
