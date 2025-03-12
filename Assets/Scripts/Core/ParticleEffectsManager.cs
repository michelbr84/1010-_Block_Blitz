using UnityEngine;

/// <summary>
/// The ParticleEffectsManager class creates and plays particle effects dynamically via script,
/// without using prefabs. The colors for each effect can be configured in the Inspector.
/// It uses the Singleton pattern to allow easy access from other scripts.
/// </summary>
public class ParticleEffectsManager : MonoBehaviour
{
    public static ParticleEffectsManager Instance { get; private set; }

    [Header("Effect Settings")]
    [Tooltip("Color for the clear line effect")]
    [SerializeField] private Color clearLineColor = Color.green;
    [Tooltip("Color for the place shape effect")]
    [SerializeField] private Color placeShapeColor = Color.blue;
    [Tooltip("Color for the game over effect")]
    [SerializeField] private Color gameOverColor = Color.red;
    [Tooltip("Color for the spawn shape effect")]
    [SerializeField] private Color spawnShapeColor = Color.yellow;

    [Tooltip("Duration for particle effects (seconds)")]
    [SerializeField] private float effectDuration = 0.5f;

    [Header("Material Settings")]
    [Tooltip("Default particle material (e.g., Particles/Standard Unlit)")]
    [SerializeField] private Material particleMaterial;

    private void Awake()
    {
        // Implement singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Se nenhum material for atribuído via Inspector, tenta usar o padrão do Unity
        if (particleMaterial == null)
        {
            particleMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        }
    }

    /// <summary>
    /// Creates and plays a particle effect at the given position using the specified color and duration.
    /// </summary>
    /// <param name="position">World position to play the effect.</param>
    /// <param name="startColor">The starting color of the particles.</param>
    /// <param name="duration">Duration of the particle effect.</param>
    private void PlayEffect(Vector3 position, Color startColor, float duration)
{
    // Cria um novo GameObject para o efeito
    GameObject effectObj = new GameObject("DynamicParticleEffect");
    effectObj.transform.position = position;
    
    // Adiciona o componente ParticleSystem
    ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
    
    // Atribui o material padrão para evitar a cor rosa
    ParticleSystemRenderer psRenderer = effectObj.GetComponent<ParticleSystemRenderer>();
    psRenderer.material = particleMaterial;

    // Desabilita Play On Awake e para o sistema para que possamos alterar as configurações
    ps.playOnAwake = false;
    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

    // Configura o módulo principal
    ParticleSystem.MainModule main = ps.main;
    main.startColor = startColor;
    main.duration = duration;
    main.loop = false;
    main.startLifetime = duration;
    main.startSpeed = 2f;
    main.startSize = 0.2f;
    main.maxParticles = 100;

    // Configura o módulo de emissão
    ParticleSystem.EmissionModule emission = ps.emission;
    emission.rateOverTime = 50f;

    // Configura o módulo de forma (shape)
    ParticleSystem.ShapeModule shape = ps.shape;
    shape.shapeType = ParticleSystemShapeType.Cone;
    shape.angle = 25f;
    shape.radius = 0.5f;

    // Limpa quaisquer partículas remanescentes e toca o efeito
    ps.Clear();
    ps.Play();

    // Destrói o objeto após o término do efeito
    Destroy(effectObj, duration + 0.5f);
}


    /// <summary>
    /// Plays the clear line particle effect at the specified position.
    /// </summary>
    public void PlayClearLineEffect(Vector3 position)
    {
        PlayEffect(position, clearLineColor, effectDuration);
    }

    /// <summary>
    /// Plays the place shape particle effect at the specified position.
    /// </summary>
    public void PlayPlaceShapeEffect(Vector3 position)
    {
        PlayEffect(position, placeShapeColor, effectDuration);
    }

    /// <summary>
    /// Plays the game over particle effect at the specified position.
    /// </summary>
    public void PlayGameOverEffect(Vector3 position)
    {
        // Game over effect duration might be longer
        PlayEffect(position, gameOverColor, effectDuration * 2);
    }

    /// <summary>
    /// Plays the spawn shape particle effect at the specified position.
    /// </summary>
    public void PlaySpawnShapeEffect(Vector3 position)
    {
        PlayEffect(position, spawnShapeColor, effectDuration);
    }
}
