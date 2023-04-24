// using System.Collections.Generic;

// public class Inventory
// {
//     private static Inventory _instance;
//     public static Inventory Instance
//     {
//         get
//         {
//             if (_instance == null) _instance = new Inventory();
//             return _instance;
//         }
//     }

//     public List<GenericPowerUp> powerUps;
//     public Inventory(){
//         powerUps = new List<GenericPowerUp>();
//     }

//     public void addPowerUp(GenericPowerUp powerUp){
//         GenericPowerUp existingPowerUp = powerUps.Find(p=>p.name == powerUp.name);
//         if (existingPowerUp != null){
//             existingPowerUp.ObtainPowerUp();
//         }else{
//             powerUps.Add(powerUp);
//             powerUp.ObtainPowerUp();
//         }
//     }
// }
