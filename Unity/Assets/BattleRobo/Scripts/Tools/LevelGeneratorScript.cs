using UnityEngine;
using UnityEngine.UI;

namespace BattleRobo.Networking
{
    public class LevelGeneratorScript : Photon.PunBehaviour
    {
        [SerializeField] private GameObject[] prefabsToLoad;
        [SerializeField] private GameObject EnvExt;
        [SerializeField] private GameObject lake;
        [SerializeField] private Transform level;
        [SerializeField] private float Height;
        [SerializeField] private int mapSize;
        [SerializeField] private int mapSpacing;
        
        private void Start()
        {
            
			//On doit avoir le même seed sinon la map est différente pour les joueurs :D
            Random.InitState(40); //seed
            
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var randomNum = Random.Range(1, prefabsToLoad.Length) - 1;

                    var platform = Instantiate(prefabsToLoad[randomNum],
                        new Vector3(j * mapSpacing, prefabsToLoad[randomNum].transform.position.y, i * mapSpacing),
                        Quaternion.identity);
                    platform.transform.Rotate(platform.transform.rotation.x, Random.Range(0,3)*90 , platform.transform.rotation.z);
                    platform.transform.parent = level;
                }
            }
            
            int m =getMapMainSize();
            EnvExt.transform.localScale= new Vector3(m,Height/4,m);
            EnvExt.transform.position= new Vector3((m-6)/2,Height/8,(m-6)/2);
            
            int l = m /5;
            lake.transform.localScale= new Vector3(l,lake.transform.localScale.y,l);
            lake.transform.position= new Vector3((m-6)/2,lake.transform.position.y,(m-6)/2);
        }

        public int getMapMainSize()
        {
            return (mapSize*mapSpacing)-mapSpacing;
        }

        public float getHeight()
        {
            return Height;
        }
    }
    
    
}