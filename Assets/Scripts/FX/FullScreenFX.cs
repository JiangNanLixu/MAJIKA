﻿using UnityEngine;
using System.Collections;

namespace MAJIKA.FX
{
    public class FullScreenFX : MonoBehaviour
    {
        private void Reset()
        {
            Update();
        }
        // Use this for initialization
        void Start()
        {
            Update();
        }

        // Update is called once per frame
        void Update()
        {
            var camera = SceneCamera.Instance.Camera;
            transform.localScale = new Vector3(SceneCamera.Instance.ViewportRect.width, SceneCamera.Instance.ViewportRect.height, 1);
            transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, transform.position.z);
        }
    }

}

