using UnityEngine;

namespace CannonTargets
{
    public static class GameStats
    {
        public static int Shots { get; private set; }
        public static int Hits { get; private set; }
        public static void RegisterShot() { Shots++; }
        public static void RegisterHit() { Hits++; }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() { Shots = Hits = 0; }
    }

    public class Hud : MonoBehaviour
    {
        private CannonController cannon;
        private GUIStyle style;
        private void Start() { cannon = FindObjectOfType<CannonController>(); }
        private void OnGUI()
        {
            if (style == null) style = new GUIStyle(GUI.skin.label) { fontSize = 20, normal = { textColor = Color.white } };
            GUI.Box(new Rect(14, 14, 460, 190), "");
            string next = cannon ? $"Следующее ядро: {cannon.NextMass:F2} кг · r {cannon.NextRadius:F3} м" : "";
            GUI.Label(new Rect(28, 22, 420, 34), next, style);
            GUI.Label(new Rect(28, 57, 440, 34), $"Выстрелы: {GameStats.Shots}    Шары сбиты: {GameStats.Hits}", style);
            GUI.Label(new Rect(28, 98, 440, 82), "WASD — движение   Q/E — поворот\n↑/↓ — поднять/опустить ствол   Space — выстрел", style);
        }
    }
}
