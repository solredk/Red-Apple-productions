using UnityEngine;
using UnityEngine.UI;

public abstract class HealthSystem : MonoBehaviour
{
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth;

    [SerializeField] protected Image frontHealthBar;
    [SerializeField] protected Image backHealthBar;

    [SerializeField] protected float lerpTimer;
    [SerializeField] protected float chipSpeed = 2f;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        //reset the lerp timer
        lerpTimer = 0;
    }

    public virtual void Heal(float healAmount)
    {
        // making sure you cant go above the max health
        if (currentHealth + healAmount > maxHealth)
        {
            healAmount = maxHealth - currentHealth;
        }
        //reset the lerp timer
        lerpTimer = 0;

        currentHealth += healAmount;
    }
    
    protected virtual void UpdateHealthUI(Color frontColour,Color backColour)
    {
        //putting the front and back fill amount in a variable
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;

        //calculating the fraction of the hitpoints
        float hFraction = currentHealth / maxHealth;

        //lerping the fill amount of the front and back health bar
        lerpTimer += Time.unscaledDeltaTime;

        //squaring the percentage complete to make the lerp more smooth
        float percentageComplete = Mathf.Clamp01(lerpTimer / chipSpeed);
        percentageComplete *= percentageComplete;

        if (fillBack > hFraction)
        {
            //set the fill amount of the front health bar to the fraction of the hitpoints
            frontHealthBar.fillAmount = hFraction;

            //lerp the fill amount of the back health bar to the fraction of the hitpoints to make sure it goes smooth
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentageComplete);

            //set the color of the back health bar to red so you are able to see that you are taking damage
            backHealthBar.color = frontColour;
        }

        else if (fillFront < hFraction)
        {
            backHealthBar.fillAmount = hFraction;

            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, backHealthBar.fillAmount, percentageComplete);

            backHealthBar.color = backColour;
        }
    }
    
    public virtual void Die()
    {
        //death logic
    }
}
