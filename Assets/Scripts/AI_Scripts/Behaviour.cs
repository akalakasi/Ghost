using UnityEngine;



public class Behaviour : MonoBehaviour
{
    public enum behaviours { IDLE, TURN_LEFT, TURN_RIGHT, WALK, RUN, POSSESSED, DEAD }
    public behaviours ai_Behaviours;
    public int delay;
}
