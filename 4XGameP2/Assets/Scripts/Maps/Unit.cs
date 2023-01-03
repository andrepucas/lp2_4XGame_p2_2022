using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    /// <summary>
    /// Constant float ratio value of particles' scale (shape).
    /// </summary>
    private const float PARTICLES_SCALE_RATIO = 0.5f;
    
    /// <summary>
    /// Constant float ratio value of particles' start size.
    /// </summary>
    private const float PARTICLES_START_SIZE_RATIO = 0.25f;

    /// <summary>
    /// Constant float ratio value of particles' gravity.
    /// </summary>
    private const float PARTICLES_SPEED_RATIO = 0.5f;

    // Serialized
    [Header("COMPONENTS")]
    [Tooltip("Icon Image component.")]
    [SerializeField] private Image _icon;
    [Tooltip("Particle system component.")]
    [SerializeField] private ParticleSystem _particles;

    /// <summary>
    /// Read only self implemented property that stores the name of the unit.
    /// </summary>
    /// <value>Name of the unit (type).</value>
    public string Name { get; }

    /// <summary>
    /// Read only self implemented property that stores all the current resources
    /// of this unit.
    /// </summary>
    /// <value>Current resources of the unit.</value>
    public IReadOnlyList<Resource> Resources => _resourceList;

    /// <summary>
    /// Private list of Resources.
    /// </summary>
    private List<Resource> _resourceList;

    private Vector2 _mapPos;

    /// <summary>
    /// Constructor method. 
    /// Sets properties' values and initializes resources list.
    /// </summary>
    /// <param name="p_name">Name.</param>
    public Unit(string p_name)
    {
        Name = p_name;

        // Initializes list.
        _resourceList = new List<Resource>();
    }

    public void Initialize(Vector2 p_mapPos)
    {
        _mapPos = p_mapPos;

        // Updates icon.
        //_icon.sprite = 

        // // Sets up particles based on parent (cell) size.
        // float m_size = transform.parent.GetComponent<RectTransform>().sizeDelta.x;

        // // Updates particles' local scale.
        // _particles.transform.localScale = Vector3.one * (m_size * PARTICLES_SCALE_RATIO);

        // // Updates particles' size and gravity modifier.
        // ParticleSystem.MainModule m_particlesMain = _particles.main;
        // m_particlesMain.startSize = m_size * PARTICLES_START_SIZE_RATIO;
        // m_particlesMain.startSpeed = m_size * PARTICLES_SPEED_RATIO;

        // // Updates particles' color.
        // Gradient m_gradient = new Gradient();
        // m_gradient.SetKeys(
        //     new GradientColorKey[]{
        //         new GradientColorKey(Color.white, 0), 
        //         new GradientColorKey(p_color, 1)},
        //     new GradientAlphaKey[]{
        //         new GradientAlphaKey(1, 0),
        //         new GradientAlphaKey(1, 1)});

        // ParticleSystem.ColorOverLifetimeModule m_particlesColor = _particles.colorOverLifetime;
        // m_particlesColor.color = m_gradient;
    }

    public void MoveTowardsTile(GameTile selectedTile)
    {

    }

    public void HarvestCurrentTile(GameTile currentTile)
    {

    }
}
