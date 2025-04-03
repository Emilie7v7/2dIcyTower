using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Parallax
{
    [System.Serializable]
    public class ParallaxBackgroundSet
    {
        public Sprite backgroundSprite;
        public float startHeight;
        public float endHeight;
    }

    public class BackgroundManager : MonoBehaviour
    {
        public GameObject backgroundPrefab;
        public Transform backgroundContainer;
        public int tileCopies = 2;
        public List<ParallaxBackgroundSet> backgroundSets;

        private readonly List<GameObject> _backgroundTiles = new();
        private Transform _player;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;

            float y = 0;
            for (var i = 0; i < tileCopies; i++)
            {
                var tile = Instantiate(backgroundPrefab, backgroundContainer);
                tile.transform.position = new Vector3(0, y, 10);
                _backgroundTiles.Add(tile);
                y += tile.GetComponent<SpriteRenderer>().bounds.size.y;
            }

            UpdateTileSprites();
        }

        private void Update()
        {
            UpdateTileSprites();
        }

        private void UpdateTileSprites()
        {
            var playerY = _player.position.y;

            foreach (var tile in _backgroundTiles)
            {
                var sr = tile.GetComponent<SpriteRenderer>();
                foreach (var set in backgroundSets)
                {
                    if (playerY >= set.startHeight && playerY <= set.endHeight)
                    {
                        if (sr.sprite != set.backgroundSprite)
                            sr.sprite = set.backgroundSprite;
                        break;
                    }
                }
            }
        }
    }
}