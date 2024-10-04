using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    //when object exit the trigger, put it to the assigned layer and sorting layers
    //used in the stair objects for player to travel between layers
    public class LayerTrigger : MonoBehaviour
    {
        public string layer;
        public string sortingLayer;

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("OrbitObject"))
            {
                other.gameObject.layer = LayerMask.NameToLayer(layer);

                // Get the SpriteRenderer of the first child only (index 0)
                if (other.transform.childCount > 0)
                {
                    Transform firstChild = other.transform.GetChild(0);
                    SpriteRenderer firstChildRenderer = firstChild.GetComponent<SpriteRenderer>();
                    if (firstChildRenderer != null)
                    {
                        firstChildRenderer.sortingLayerName = sortingLayer;
                    }
                }
            }
        }
    }
}
