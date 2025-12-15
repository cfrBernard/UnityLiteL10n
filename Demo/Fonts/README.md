# Font Setup for Localization Demo

This folder contains the necessary fonts for the multi-language demo (JA / KO).

To properly display Japanese (JA) and Korean (KO) text in Unity with TextMeshPro, you just need to add these fonts as fallbacks to your default font.

## Steps to Configure Font Fallbacks

1. **Ensure Fonts Are Imported**
   - Make sure you have the fonts `.ttf` or `.otf` files in the `Fonts/` folder.
   - In Unity, ensure each font is set to **Dynamic** and **Include Font Data** is enabled.

2. **Add JA/KO Fallbacks**
   - Select your **main TMP Font Asset** (e.g., Latin font like "Liberation Sans").
   - In the Inspector, go to the **Fallback Font Asset Table**.
   - Click the `+` button and add the TMP Font Assets for JA and KO.
   - Save the changes.

3. **Test**
   - Now your texts in EN/FR/ES will display correctly.
   - JA and KO characters will automatically fall back to their respective fonts.

> Using a **Dynamic Latin font** (e.g., Liberation Sans) as your primary font asset is recommended for simplicity and faster setup.
> With this setup, there's no need to swap fonts dynamically in the code for the demo. It just works!

---
