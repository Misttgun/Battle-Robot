using UnityEngine;

namespace BattleRobo
{
    public class LevelGeneratorScript : Photon.PunBehaviour
    {
        [SerializeField]
        private GameObject[] prefabsToLoad;

        [SerializeField]
        private GameObject envExt;

        [SerializeField]
        private GameObject lake;

        [SerializeField]
        private Transform level;

        [SerializeField]
        private float height;

        [SerializeField]
        private int mapSize;

        [SerializeField]
        private int mapSpacing;

        private void Start()
        {
            //On doit avoir le même seed sinon la map est différente pour les joueurs :D
            Random.InitState(40); //seed

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var randomNum = Random.Range(1, prefabsToLoad.Length) - 1;

                    var platform = Instantiate(prefabsToLoad[randomNum], new Vector3(j * mapSpacing, prefabsToLoad[randomNum].transform.position.y, i * mapSpacing), Quaternion.identity);
                    platform.transform.Rotate(platform.transform.rotation.x, Random.Range(0, 3) * 90, platform.transform.rotation.z);
                    platform.transform.parent = level;
                }
            }

            int mMapSize = getMapMainSize();
            float halfMapSize = (float)mMapSize / 2;
            envExt.transform.localScale = new Vector3(mMapSize, height, mMapSize);
            envExt.transform.position = new Vector3(halfMapSize, -height / 5, halfMapSize);


            lake.transform.localScale = new Vector3(mMapSize, lake.transform.localScale.y, mMapSize);
            lake.transform.position = new Vector3(halfMapSize, lake.transform.position.y, halfMapSize);
        }

        public int getMapMainSize()
        {
            return mapSize * mapSpacing - mapSpacing;
        }

        public float getHeight()
        {
            return height;
        }
    }
}