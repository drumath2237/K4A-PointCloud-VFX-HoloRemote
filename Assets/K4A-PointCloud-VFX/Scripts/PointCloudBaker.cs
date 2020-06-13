using System;
using System.Linq;
using UnityEngine;

namespace K4APointCloudVFX
{
    [RequireComponent(typeof(AzureKinectPointCloudSensor))]
    public class PointCloudBaker : MonoBehaviour
    {
        [SerializeField] private RenderTexture _positionMap=default;
        [SerializeField] private ComputeShader _vertexBaker;

        private AzureKinectPointCloudSensor _sensor;

        private ComputeBuffer _positionBuffer;
        private RenderTexture _tmpPositionMap;
        private int _vertexCountID, _transformID, _positionBufferID, _positionMapID;

        private void Start()
        {
            _sensor = GetComponent<AzureKinectPointCloudSensor>();
            
            _vertexCountID = Shader.PropertyToID("VertexCount");
            _transformID = Shader.PropertyToID("Transform");
            _positionBufferID = Shader.PropertyToID("PositionBuffer");
            _positionMapID = Shader.PropertyToID("PositionMap");

            var vertexCount = _sensor.Count;
            _positionBuffer = new ComputeBuffer(vertexCount * 3, sizeof(float));
            Graphics.CopyTexture(_positionMap, _tmpPositionMap);
        }

        private void Update()
        {
            if (_positionBuffer == null) return;
            
            var mapWidth = _positionMap.width;
            var mapHeight = _positionMap.height;
            var vCount = _sensor.Count;

            float[] bufferArray = _sensor.Vertices.Select(v => new float[] {v.x, v.y, v.z}).Cast<float>().ToArray();
            _positionBuffer.SetData(bufferArray);
            
            _vertexBaker.SetInt(_vertexCountID, vCount);
            _vertexBaker.SetMatrix(_transformID, gameObject.transform.localToWorldMatrix);
            _vertexBaker.SetBuffer(0, _positionBufferID, _positionBuffer);
            _vertexBaker.SetTexture(0, _positionMapID, _tmpPositionMap);
            
            _vertexBaker.Dispatch(0, mapWidth / 8, mapHeight / 8, 1);

            Graphics.CopyTexture(_tmpPositionMap, _positionMap);
        }

        private void OnDestroy()
        {
//            _positionBuffer.Dispose();
//            _tmpPositionMap = null;
        }
    }
}