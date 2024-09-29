using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public GameObject plantModel;

    //public Texture deadPlant;
    public Texture alivePlant;
    public Material plantMaterial;

    public bool hasGrown = false;
    public bool fullyGrown = false;

    public int blendShapeIndex = 0;
    public float blendShapeWeightFrom = 0f;
    public float blendShapeWeightTo = 100f;
    public float transitionDuration = 3f; // duration of growth in seconds

    private Renderer plantRenderer; // for texture swap
    private SkinnedMeshRenderer plantSkinMeshRender; // for blend shape

    public void grow()
    {
        hasGrown = true;
        Debug.Log("plant has grown");
        //add code to grow plant
        if (plantModel != null)
        {
            plantRenderer = plantModel.GetComponent<Renderer>();
            plantSkinMeshRender = plantModel.GetComponent<SkinnedMeshRenderer>();

            plantMaterial = plantRenderer.material;

            if (plantSkinMeshRender != null)
            {
                growPlantOverTime(transitionDuration);                
            }
            plantRenderer.material.mainTexture = alivePlant;
        }
    }

    public void growPlantOverTime(float duration)
    {
        float elapsedTime = 0f;
        float blendFactor = 0f;
        float currentBlendWeight = 0f;

        while(!fullyGrown)
        {
            blendFactor = elapsedTime / duration;

            plantMaterial.SetFloat("_Blend", blendFactor);

            currentBlendWeight = Mathf.Lerp(blendShapeWeightFrom, blendShapeWeightTo, blendFactor);
            plantSkinMeshRender.SetBlendShapeWeight(blendShapeIndex, currentBlendWeight);
    
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= duration && blendFactor >= 1.0f)
            {
                fullyGrown = true;
            }
        }
    }
}