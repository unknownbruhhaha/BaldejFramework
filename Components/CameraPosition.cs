﻿using System;
using GameEngineAPI.Components;
using GameEngineAPI.Render;

namespace GameEngineAPI.Components
{
    public class CameraPosition : Component
    {
        public string componentID { get => ""; }
        public GameObject? owner { get; set; }

        public void OnUpdate()
        {
            Transform3D transform = (Transform3D)owner.GetComponent("Transform");
            Render.Render.camera.Position = transform.Position;
        }
    }
}
