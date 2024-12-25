using UnityEngine;

public class ClassManager : MonoBehaviour
{
    public enum ClassType { Human, Elf, Dwarf }

    [Header("Class Counts")]
    public int humanCount = 0;
    public int elfCount = 0;
    public int dwarfCount = 0;

    [Header("Maximum Counts")]
    [SerializeField] private int maxHumanCount = 8;
    [SerializeField] private int maxElfCount = 5;
    [SerializeField] private int maxDwarfCount = 6;

    public void AddClassCount(ClassType classType)
    {
        switch (classType)
        {
            case ClassType.Human:
                if (humanCount < maxHumanCount)
                {
                    humanCount++;
                    Debug.Log($"Human count increased to {humanCount}");
                }
                else
                {
                    Debug.LogWarning("Cannot add more humans. Max limit reached!");
                }
                break;

            case ClassType.Elf:
                if (elfCount < maxElfCount)
                {
                    elfCount++;
                    Debug.Log($"Elf count increased to {elfCount}");
                }
                else
                {
                    Debug.LogWarning("Cannot add more elves. Max limit reached!");
                }
                break;

            case ClassType.Dwarf:
                if (dwarfCount < maxDwarfCount)
                {
                    dwarfCount++;
                    Debug.Log($"Dwarf count increased to {dwarfCount}");
                }
                else
                {
                    Debug.LogWarning("Cannot add more dwarves. Max limit reached!");
                }
                break;

            default:
                Debug.LogError("Unknown class type!");
                break;
        }
    }
}
