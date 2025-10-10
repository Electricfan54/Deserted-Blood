using UnityEngine;

public interface IEffect
{
    public void ApplyBurnEffect(float duration, int tickDamage, float tickRate);
    public void ApplyFreezeEffect(float duration);

    // Add stun function
}
