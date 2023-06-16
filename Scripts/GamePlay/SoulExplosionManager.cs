using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SoulExplosionManager : MonoBehaviour
{
    [SerializeField] GameObject _soulExplosionPrefab = null;
    [SerializeField] Transform _soulTransform = null;
    [SerializeField] Transform _soulExplosionsTransform = null;
    [SerializeField] int _maxSouls = 30; // max number of soul particles we can pool
    [SerializeField] private float _soulChance = 0.1f; // The chance a soul will spawn
    [SerializeField] private float _resetTime = 1.0f; // Time to unlock a trnasform location for a soul to spawn at
    [SerializeField] private Vector3 _smallSoulScale = Vector3.one; // Small soul size
    [SerializeField] private Vector3 _bigSoulScale = new Vector3(2.0f, 2.0f, 2.0f); // Big soul size
    [SerializeField] private List<Color> _soulColors = new List<Color>();
    [SerializeField] private List<Transform> _soulLocations = new List<Transform>();
    private List<SoulExplosion> _soulExplosions = new List<SoulExplosion>();
    private List<bool> _transformHasSoul = new List<bool>();
    private int _beatCount = 0;

    private void Awake()
    {
        LevelLoader.CallOnComplete(Initialize);
    }

    private void OnDisable()
    {
        var bus = ServiceLocator.Get<EventBusCallbacks>();
        bus.OnMusicBeatMessageHandled -= OnBeat;
    }

    private void Initialize()
    {
        var bus = ServiceLocator.Get<EventBusCallbacks>();
        bus.OnMusicBeatMessageHandled += OnBeat;

        for (int i = 0; i < _soulLocations.Count; ++i)
        {
            _transformHasSoul.Add(false);
        }
    }

    public void OnBeat(OnMusicBeatMessage message)
    {
        ++_beatCount;
        if(_beatCount >= 4)
        {
            _beatCount = 0;
            PlaySoul(true);
            return;
        }

        PlaySoul(false);
    }

    private void PlaySoul(bool bigSoul)
    {
        for(int i = 0; i < _transformHasSoul.Count; ++i)
        {
            if (_transformHasSoul[i] == true || Random.value > _soulChance)
            {
                continue;
            }

            _transformHasSoul[i] = true;

            // Move and play a free soul
            bool spawnedSoul = false;
            foreach (var soulExplosion in _soulExplosions)
            {
                if (soulExplosion.SoulReleasePS.IsAlive(true) == false)
                {
                    PlaySoulParticle(soulExplosion, _soulLocations[i].position, bigSoul ? _bigSoulScale : _smallSoulScale);
                    spawnedSoul = true;
                    break;
                }
            }

            StartCoroutine(ResetTransformHasSoul(i));
            if (spawnedSoul || _soulExplosions.Count >= _maxSouls)
            {
                continue;
            }

            // None available make more
            var _soulExplosionObj = Instantiate(_soulExplosionPrefab);
            _soulExplosionObj.transform.parent = _soulExplosionsTransform;
            var newSoulExplosion = _soulExplosionObj.GetComponent<SoulExplosion>();

            PlaySoulParticle(newSoulExplosion, _soulLocations[i].position, bigSoul ? _bigSoulScale : _smallSoulScale);
            _soulExplosions.Add(newSoulExplosion);
        }
    }

    private void PlaySoulParticle(SoulExplosion soulExplosion, Vector3 newPos, Vector3 newScale)
    {
        soulExplosion.transform.position = newPos;
        soulExplosion.transform.localScale = newScale;

        Color newSoulColor = _soulColors[Random.Range(0, _soulColors.Count)];
        var colorMain = soulExplosion.SoulReleasePS.main;
        colorMain.startColor = newSoulColor;
        colorMain = soulExplosion.StarPS.main;
        colorMain.startColor = newSoulColor;
        colorMain = soulExplosion.PoofPS.main;
        colorMain.startColor = newSoulColor;

        soulExplosion.SoulReleasePS.Play();
        soulExplosion.StarPS.Play();
        soulExplosion.PoofPS.Play();
    }

    private IEnumerator ResetTransformHasSoul(int index)
    {
        yield return new WaitForSeconds(_resetTime);
        _transformHasSoul[index] = false;
    }


#if UNITY_EDITOR
    public void FixSoulTransforms()
    {
        _soulLocations.Clear();

        for (int i = 0; i < _soulTransform.childCount; ++i)
        {
            var newLocation = _soulTransform.GetChild(i).transform;
            _soulLocations.Add(newLocation);
        }
        EditorUtility.SetDirty(this);
    }
#endif

}
