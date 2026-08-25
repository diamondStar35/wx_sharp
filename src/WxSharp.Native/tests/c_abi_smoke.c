#include "../wxsharp.h"

int main(void)
{
    /* Loading and calling this side-effect-free function verifies that the
       public header is valid C and the exported C symbol links correctly. */
    (void)wxsharp_custom_accessibility_available();
    return 0;
}
