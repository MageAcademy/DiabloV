using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PROS.Water
{
    public class Water : MonoBehaviour
    {
        public RenderTexture currentHeightTexture = null;

        public RenderTexture lastHeightTexture = null;

        public RenderTexture nextHeightTexture = null;

        public ComputeShader water = null;

        [Range(0f, 10f)] public float deltaSize = 0.1f;

        [Range(0f, 10f)] public float waveParameter = 1f;

        [Range(0f, 10f)] public float waveSpeed = 3f;

        private int hashDeltaTime = Shader.PropertyToID("deltaTime");

        private int hashWavePosition = Shader.PropertyToID("wavePosition");

        private Kernel kernelInitialize = null;

        private Kernel kernelAddWave = null;

        private Kernel kernelUpdate = null;

        private Kernel kernelAfterUpdate = null;

        private int textureSize = 256;

        private Dictionary<Collider, Vector3> triggerDictionary = new Dictionary<Collider, Vector3>();

        private float waveSign = 1f;


        private void Start()
        {
            currentHeightTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.RFloat)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Trilinear,
                wrapMode = TextureWrapMode.Clamp,
            };
            currentHeightTexture.Create();
            lastHeightTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.RFloat)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Trilinear,
                wrapMode = TextureWrapMode.Clamp,
            };
            lastHeightTexture.Create();
            nextHeightTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.RFloat)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Trilinear,
                wrapMode = TextureWrapMode.Clamp,
            };
            nextHeightTexture.Create();
            kernelInitialize = new Kernel(water, "Initialize");
            kernelAddWave = new Kernel(water, "AddWave");
            kernelUpdate = new Kernel(water, "Update");
            kernelAfterUpdate = new Kernel(water, "AfterUpdate");
            water.SetFloat("deltaSize", deltaSize);
            water.SetFloat("textureSize", textureSize);
            water.SetFloat("waveParameter", waveParameter);
            water.SetTexture(kernelInitialize.index, "currentHeightTexture", currentHeightTexture);
            water.SetTexture(kernelInitialize.index, "lastHeightTexture", lastHeightTexture);
            water.SetTexture(kernelAddWave.index, "currentHeightTexture", currentHeightTexture);
            water.SetTexture(kernelUpdate.index, "currentHeightTexture", currentHeightTexture);
            water.SetTexture(kernelUpdate.index, "lastHeightTexture", lastHeightTexture);
            water.SetTexture(kernelUpdate.index, "nextHeightTexture", nextHeightTexture);
            water.SetTexture(kernelAfterUpdate.index, "currentHeightTexture", currentHeightTexture);
            water.SetTexture(kernelAfterUpdate.index, "lastHeightTexture", lastHeightTexture);
            water.SetTexture(kernelAfterUpdate.index, "nextHeightTexture", nextHeightTexture);
            kernelInitialize.Invoke(textureSize);
            GetComponent<Renderer>().material.SetTexture("_HeightMap", nextHeightTexture);
            ModifyMesh();
        }


        private void FixedUpdate()
        {
            water.SetFloat(hashDeltaTime, Time.fixedDeltaTime * waveSpeed);
            kernelUpdate.Invoke(textureSize);
            kernelAfterUpdate.Invoke(textureSize);
        }


        private void OnTriggerStay(Collider other)
        {
            Vector3 position = other.transform.position;
            if (triggerDictionary.ContainsKey(other) && Vector3.Distance(triggerDictionary[other], position) < 0.4f)
            {
                return;
            }

            Vector3 velocity = Vector3.up * Random.Range(4f, 6f) * waveSign;
            triggerDictionary[other] = position;
            waveSign *= -1f;
            AddWave(triggerDictionary[other], velocity);
        }


        private void AddWave(Vector3 position, Vector3 velocity)
        {
            var localPosition = transform.worldToLocalMatrix.MultiplyPoint(position) * 2f;
            var localVelocity = transform.worldToLocalMatrix.MultiplyVector(velocity);
            water.SetFloats(hashWavePosition, localPosition.x, localPosition.y, localVelocity.z);
            kernelAddWave.Invoke(textureSize);
        }


        private void ModifyMesh()
        {
            int count = 256;
            var subDivIndices = new int[6 * count * count];
            var subDivVerts = new Vector3[4 * count * count];
            var subDivUvs = new Vector2[4 * count * count];
            var edgeLength = 1.0f / count;
            for (int xIndex = 0; xIndex < count; xIndex++)
            {
                var offsetX = edgeLength * xIndex;
                for (int yIndex = 0; yIndex < count; yIndex++)
                {
                    var offsetY = edgeLength * yIndex;
                    var offsetIndex = count * xIndex + yIndex;

                    var leftBottom = new Vector3(offsetX - 0.5f, offsetY - 0.5f);
                    var rightBottom = leftBottom + new Vector3(edgeLength, 0);
                    var leftUp = leftBottom + new Vector3(0, edgeLength);
                    var rightUp = leftBottom + new Vector3(edgeLength, edgeLength);

                    subDivVerts[4 * offsetIndex + 0] = leftBottom;
                    subDivVerts[4 * offsetIndex + 1] = rightBottom;
                    subDivVerts[4 * offsetIndex + 2] = leftUp;
                    subDivVerts[4 * offsetIndex + 3] = rightUp;

                    var uvLeftBottom = new Vector2(offsetX, offsetY);
                    var uvRightBottom = uvLeftBottom + new Vector2(edgeLength, 0);
                    var uvLeftUp = uvLeftBottom + new Vector2(0, edgeLength);
                    var uvRightUp = uvLeftBottom + new Vector2(edgeLength, edgeLength);

                    subDivUvs[4 * offsetIndex + 0] = uvLeftBottom;
                    subDivUvs[4 * offsetIndex + 1] = uvRightBottom;
                    subDivUvs[4 * offsetIndex + 2] = uvLeftUp;
                    subDivUvs[4 * offsetIndex + 3] = uvRightUp;

                    subDivIndices[6 * offsetIndex + 0] = 4 * offsetIndex + 0;
                    subDivIndices[6 * offsetIndex + 1] = 4 * offsetIndex + 3;
                    subDivIndices[6 * offsetIndex + 2] = 4 * offsetIndex + 1;
                    subDivIndices[6 * offsetIndex + 3] = 4 * offsetIndex + 3;
                    subDivIndices[6 * offsetIndex + 4] = 4 * offsetIndex + 0;
                    subDivIndices[6 * offsetIndex + 5] = 4 * offsetIndex + 2;
                }
            }

            var subDivMesh = new Mesh();
            subDivMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            subDivMesh.name = "Water Mesh";
            subDivMesh.SetVertices(subDivVerts);
            subDivMesh.SetTriangles(subDivIndices, 0);
            subDivMesh.SetUVs(0, subDivUvs);
            subDivMesh.RecalculateBounds();
            subDivMesh.RecalculateNormals();
            subDivMesh.RecalculateTangents();
            GetComponent<MeshFilter>().mesh = subDivMesh;
        }
    }
}