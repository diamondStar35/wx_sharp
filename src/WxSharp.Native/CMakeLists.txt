cmake_minimum_required(VERSION 3.22)

project(WxSharp.Native VERSION 0.1.0 LANGUAGES C CXX)

option(WXSHARP_STATIC_MSVC_RUNTIME
  "Link the MSVC runtime statically into wxsharp and its test executable"
  OFF)

if(WIN32)
  enable_language(RC)
endif()

# Prefer a package configuration, then fall back to CMake's FindwxWidgets
# module for prebuilt archives and system installations.
find_package(wxWidgets 3.0 CONFIG QUIET COMPONENTS core base)
if(wxWidgets_FOUND)
  if(TARGET wx::core AND TARGET wx::base)
    set(WXSHARP_WX_TARGETS wx::core wx::base)
  elseif(TARGET wxWidgets::wxWidgets)
    set(WXSHARP_WX_TARGETS wxWidgets::wxWidgets)
  else()
    message(FATAL_ERROR "The wxWidgets package was found but did not export supported CMake targets.")
  endif()
else()
  find_package(wxWidgets 3.0 REQUIRED MODULE COMPONENTS core base)
  if(TARGET wxWidgets::wxWidgets)
    set(WXSHARP_WX_TARGETS wxWidgets::wxWidgets)
  else()
    include(${wxWidgets_USE_FILE})
    set(WXSHARP_WX_TARGETS ${wxWidgets_LIBRARIES})
  endif()
endif()

add_library(wxsharp SHARED
  app.cpp
  window.cpp
  dialog.cpp
  control.cpp
  control_props.cpp
  accessible.cpp
  sizer.cpp
  label.cpp
  button.cpp
  textbox.cpp
  checkbox.cpp
  radiobutton.cpp
  slider.cpp
  choice.cpp
  listbox.cpp
  canvas.cpp
  layout.cpp
  services.cpp)

if(WIN32)
  target_sources(wxsharp PRIVATE wxsharp.rc)
endif()

target_compile_features(wxsharp PRIVATE cxx_std_17)
target_compile_definitions(wxsharp PRIVATE WXSHARP_BUILD)
target_link_libraries(wxsharp PRIVATE ${WXSHARP_WX_TARGETS})

set_target_properties(wxsharp PROPERTIES
  CXX_EXTENSIONS OFF
  CXX_VISIBILITY_PRESET hidden
  VISIBILITY_INLINES_HIDDEN YES
  OUTPUT_NAME wxsharp)

if(MSVC)
  if(WXSHARP_STATIC_MSVC_RUNTIME)
    set(_wxsharp_msvc_runtime "MultiThreaded$<$<CONFIG:Debug>:Debug>")
  else()
    set(_wxsharp_msvc_runtime "MultiThreaded$<$<CONFIG:Debug>:Debug>DLL")
  endif()
  set_property(TARGET wxsharp PROPERTY
    MSVC_RUNTIME_LIBRARY "${_wxsharp_msvc_runtime}")
  target_compile_options(wxsharp PRIVATE /W4 /permissive- /utf-8)
else()
  target_compile_options(wxsharp PRIVATE -Wall -Wextra -Wpedantic)
endif()

install(TARGETS wxsharp
  RUNTIME DESTINATION bin
  LIBRARY DESTINATION lib
  ARCHIVE DESTINATION lib)
install(FILES wxsharp.h DESTINATION include)

include(CTest)
if(BUILD_TESTING)
  add_executable(wxsharp_c_abi_smoke tests/c_abi_smoke.c)
  if(MSVC)
    set_property(TARGET wxsharp_c_abi_smoke PROPERTY
      MSVC_RUNTIME_LIBRARY "${_wxsharp_msvc_runtime}")
  endif()
  target_link_libraries(wxsharp_c_abi_smoke PRIVATE wxsharp)
  add_test(NAME wxsharp.c-abi COMMAND wxsharp_c_abi_smoke)
endif()
