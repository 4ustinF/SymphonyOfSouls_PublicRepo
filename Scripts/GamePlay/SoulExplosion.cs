using UnityEngine;

public class SoulExplosion : MonoBehaviour
{
    [SerializeField] private ParticleSystem _soulReleasePS = null;
    [SerializeField] private ParticleSystem _starPS = null;
    [SerializeField] private ParticleSystem _poofPS = null;

    public ParticleSystem SoulReleasePS => _soulReleasePS;
    public ParticleSystem StarPS => _starPS;
    public ParticleSystem PoofPS => _poofPS;
}
