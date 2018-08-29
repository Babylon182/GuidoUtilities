using Events;
using UnityEngine;

public class DumbEnemy : MonoBehaviour {

    public FloatReference maxHp;
    public FloatReference moveSpeed;

    private void Awake()
    {
        EventsManager<EventHeroDamaged>.SubscribeToEvent(HeroDamage);
        EventsManager<EventHeroDamaged>.SubscribeToEvent(HeroTestingDamage);
        EventsManager<EventHeroDamaged>.SubscribeToEvent(HeroAnAnotherOneDamage);
        EventsManager<EventHeroDead>.SubscribeToEvent(HeroDead);
    }

    private void HeroAnAnotherOneDamage(EventHeroDamaged dataEvent)
    {
        Debug.Log("HeroAnAnotherOneDamage: " + dataEvent.damage);
        Debug.Log("HeroAnAnotherOneDamage: " + dataEvent.knockBack);  
    }

    private void HeroTestingDamage(EventHeroDamaged dataEvent)
    {
        Debug.Log("HeroTestingDamage: " + dataEvent.damage);
        Debug.Log("HeroTestingDamage: " + dataEvent.knockBack);  
    }

    private void HeroDead(EventHeroDead dataEvent)
    {
        Debug.Log(dataEvent.score);
        Debug.Log(dataEvent.continuesLeft);   
    }

    private void HeroDamage(EventHeroDamaged dataEvent)
    {
        Debug.Log("HeroDamage: " +dataEvent.damage);
        Debug.Log("HeroDamage: " +dataEvent.knockBack);   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EventHeroDamaged data = new EventHeroDamaged(10, new Vector3(45,45,45));
            EventsManager<EventHeroDamaged>.TriggerEvent(data);
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            EventHeroDead data = new EventHeroDead(10000, 2);
            EventsManager<EventHeroDead>.TriggerEvent(data);

        }
    }

    public void CheckState()
    {
        Debug.Log("My life is " + maxHp.Value);
        Debug.Log("My speed is " + moveSpeed.Value);
    }
}
