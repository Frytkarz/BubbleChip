using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BubbleChip
{
    [Serializable]
    public class BallConfiguration
    {
//        public static readonly BallConfiguration[] ballConfigurations = {
//            new BallConfiguration("ORANGE", new Color32(255, 148, 108, 255)),
//            new BallConfiguration("GREEN", new Color32(214, 255, 142, 255)),
//            new BallConfiguration("BLUE", new Color32(162, 174, 255, 255)),
//            new BallConfiguration("WHITE", new Color(1, 1, 1)),
//            new BallConfiguration("GREY", new Color32(152, 152, 152, 255)),
//            new BallConfiguration("MAGENTA", new Color32(255, 0, 255, 255)),
//        };

//        public static BallConfiguration RandomConfiguration
//        {
//            get
//            {
//                return ballConfigurations[Random.Range(0, ballConfigurations.Length)];
//            }
//        }

        public string name;
        public Material material;
        //public Color color;

        public BallConfiguration()
        {
        }

        public BallConfiguration(string name, Material material)
        {
            this.name = name;
            this.material = material;
        }


//        public BallConfiguration(string name, Color color)
//        {
//            this.name = name;
//            this.color = color;
//        }
    }
}