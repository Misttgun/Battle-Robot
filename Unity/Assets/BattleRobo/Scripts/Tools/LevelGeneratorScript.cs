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

        public int mapSize;

        public int mapSpacing;

        private int seed;

        private void Awake()
        {
            seed = PhotonNetwork.inRoom ? System.Convert.ToInt32(PhotonNetwork.room.CustomProperties["seed"]) : 40;
            
            //On doit avoir le même seed sinon la map est différente pour les joueurs :D
            Random.InitState(seed); //seed
            
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var randomNum = Random.Range(0, prefabsToLoad.Length);
                    var platform = Instantiate(prefabsToLoad[randomNum], new Vector3(j * mapSpacing, prefabsToLoad[randomNum].transform.position.y, i * mapSpacing), Quaternion.identity);
                    platform.transform.parent = level;
                }
            }

            int mMapSize = getMapMainSize();
            float halfMapSize = (float) mMapSize / 2;
            envExt.transform.localScale = new Vector3(mMapSize, height, mMapSize);
            envExt.transform.position = new Vector3(halfMapSize, -height / 5, halfMapSize);


            lake.transform.localScale = new Vector3(mMapSize, lake.transform.localScale.y, mMapSize);
            lake.transform.position = new Vector3(halfMapSize, lake.transform.position.y, halfMapSize);
        }

        public int getMapMainSize()
        {
            return mapSize * mapSpacing;
        }

        public float getHeight()
        {
            return height;
        }
    }
}