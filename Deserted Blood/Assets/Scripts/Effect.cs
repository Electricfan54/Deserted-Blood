using UnityEngine;

public class Effect : MonoBehaviour
{

    public enum EffectType
    {
        Burn,
        Freeze,
        Stun,
    }
    public EffectType type;
    public float duration;
    public int tickDamage;
    public float tickRate;

    private void OnTriggerEnter(Collider other)
    {
        IEffect effect = other.GetComponent<IEffect>();
        if (effect != null)
        {
            switch (type)
            {
                case EffectType.Burn:
                    effect.ApplyBurnEffect(duration, tickDamage, tickRate);
                    break;
                case EffectType.Freeze:
                    effect.ApplyFreezeEffect(duration);
                    break;
                case EffectType.Stun:
                    //Add stun function call
                    effect.ApplyStunEffect(duration);
                    break;
            }
        }
    }
}
