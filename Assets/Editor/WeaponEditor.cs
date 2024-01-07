//we need that using statement
using UnityEditor;
using UnityEngine;

//We connect the editor with the Weapon SO class
[CustomEditor(typeof(WeaponSO))]
//We need to extend the Editor
public class WeaponEditor : Editor
{
    //Here we grab a reference to our Weapon SO
    WeaponSO weapon;

    private void OnEnable()
    {
        //target is by default available for you
        //because we inherite Editor
        weapon = target as WeaponSO;
    }

    //Here is the meat of the script
    public override void OnInspectorGUI()
    {
        //Guard clause
        if (weapon.sprite == null)
            return;

        //Convert the weaponSprite (see SO script) to Texture
        Texture2D texture = AssetPreview.GetAssetPreview(weapon.sprite);
        //We crate empty space 80x80 (you may need to tweak it to scale better your sprite
        //This allows us to place the image JUST UNDER our default inspector
        if (texture == null) return;
        GUILayout.Label("", GUILayout.Height(80), GUILayout.Width((texture.width * 80) / texture.height));
        //Draws the texture where we have defined our Label (empty space)
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
        //Draw whatever we already have in SO definition
        base.OnInspectorGUI();
    }
}
