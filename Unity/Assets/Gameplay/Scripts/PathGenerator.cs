using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using BattleRobo;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    [SerializeField] private LevelGeneratorScript mapGenerator;
    [SerializeField] private Transform[] pathVehicle;
    [SerializeField] private GameObject vehicle;
    [SerializeField] private float speed;
    private Transform vehicleTarget;
    private float m;
    private int c;

    private void Start()
    {
        pathTransform(); 
        
    }

    private void Update()
    { 
        // Bouger le vehicule sur le prochain target point
        Vector3 direction = vehicleTarget.position - vehicle.transform.position;
        vehicle.transform.Translate(direction.normalized*speed*Time.deltaTime, Space.World);  
       
        // changer de target point
        if (Vector3.Distance(vehicle.transform.position, vehicleTarget.position) <= 0.9f)                               
        {
            if (c >= pathVehicle.Length)
            {
                //Detruit le bus après un tour complet 
                vehicle.SetActive(false);                                                                                       
                return;
            }    
            c++;
            vehicleTarget = pathVehicle[c];
        }
    }
    
    // placement du chemin du vehicule en fon ction de la taille de la map 
    private void pathTransform()
    {
        m = mapGenerator.getMapMainSize();
  
        vehicle.transform.position = new Vector3(0, 40 , 0);
        
        pathVehicle[0].position = new Vector3(0 , 40 , 0);
        pathVehicle[1].position = new Vector3(0 , 40 , m);
        pathVehicle[2].position = new Vector3(m , 40 , m);
        pathVehicle[3].position = new Vector3(m , 40 , 0);
        
        vehicleTarget = pathVehicle[0];
    }



}
