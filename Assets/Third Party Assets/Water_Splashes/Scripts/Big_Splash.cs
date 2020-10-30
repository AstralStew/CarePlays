using UnityEngine;
using System.Collections;

public class Big_Splash : MonoBehaviour {


    public GameObject BigSplash;

    private float splashFlag = 0;

    [SerializeField] bool activateOnEnable;


    void OnEnable (){

        if (activateOnEnable) {
            ActivateParticles();
        }
        else BigSplash.SetActive(false);
    }

    public void ActivateParticles (){

        if (splashFlag == 0) {
		    StartCoroutine("TriggerSplash");
        }
       
    }
    
    

   
	IEnumerator TriggerSplash (){
    
        splashFlag = 1;
    
        BigSplash.SetActive(true);

	    yield return new WaitForSeconds (3.5f);

        BigSplash.SetActive(false);

        splashFlag = 0;

    }




}