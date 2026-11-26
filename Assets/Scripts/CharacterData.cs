
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewCharacter",
    menuName = "CastleWars/Character")]
public class CharacterData : ScriptableObject
{
    [Header("Character Information")]
    public CharacterType characterType;
    public string characterName;

    [Header("Combat Stats")]
    public int maxHealth = 100;
    public int attackDamage = 20;
    public float moveSpeed = 3f;
    public float attackInterval = 1f;
    public float attackRange = 1.5f;
    public float detectionRange = 5f;

    [Header("Ranged Attack")]
    public Projectile projectilePrefab;
    public CameraFlash cameraFlashPrefab;

    [Header("Special Ability")]
    public float specialCooldown = 10f;
    public CarAttack carPrefab;

    [Header("Visuals")]
    public GameObject characterModel;
    public Sprite characterIcon;
}
