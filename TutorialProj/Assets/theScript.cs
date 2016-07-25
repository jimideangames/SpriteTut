using UnityEngine;
using System.Collections;

public class theScript : MonoBehaviour
{
    public int width, height;
    public string seed;
    public bool useRandomSeed;
    [Range(40, 70)]
    public int randomFillPercent; //54 is a good start
    [Range(0, 10)]
    public int smoother; // 4 is a good start
    // This gameobject needs to have a SpriteRenderer
    public GameObject go;
    public Texture2D[] tiles;
    // the map
    int[,] wallMap;
    // Use this for initialization
    void Start ()
    {
        if (!go.GetComponent<SpriteRenderer>())
        {
            Debug.LogError(go.name + " does not have a SpriteRenderer!");
        }
        else
        {
            CreateMap();
            DrawMap(go, tiles, wallMap, 0);
        }
	}

    void CreateMap()
    {
        wallMap = new int[width, height];
        wallMap = RandomFillMap();

        //
        for (int i = 0; i < smoother; i++)
        {
            SmoothMap();
        }
    }
    int[,] RandomFillMap()
    {
        int[,] genericMap = new int[width, height];
        if (useRandomSeed)
        {
            seed += Time.time.ToString();
        }
        System.Random rand1 = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    genericMap[x, y] = 1;
                }
                else
                {
                    genericMap[x, y] = (rand1.Next(0, 100) > randomFillPercent) ? 1 : 0;
                }
            }
        }
        return genericMap;
    }
    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                //
                if (neighbourWallTiles > 4)
                {
                    wallMap[x, y] = 1;
                }
                //
                else if (neighbourWallTiles < 4)
                {
                    wallMap[x, y] = 0;
                }
            }
        }
    }
    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        //
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += wallMap[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    /// <summary>
    /// go is the object you want to use to create in the scene. This must have a SpriteRenderer.
    /// variant is a Texture2D array to allow randomness to your map (these are set to 32 pixels square hard coded but can be changed to your size)
    /// map is an instance of the wallMap created. This allows for several variations of the map to be created that can be filled with different textures.
    /// depth is used for the layering of several textures.
    /// </summary>
    /// <param name="go"></param>
    /// <param name="variant"></param>
    /// <param name="map"></param>
    /// <param name="depth"></param>
    void DrawMap(GameObject go, Texture2D[] variant, int[,] map, int depth)
    {
        Texture2D mapTexture;
        GameObject theTextureGO = Instantiate(go) as GameObject;
        //rend = theTextureGO.GetComponent<SpriteRenderer>();
        mapTexture = new Texture2D(variant[0].width * width, variant[0].height * height); /// THIS ASSUMES EVERY TEXTURE IN YOUR VARIANT IS THE SAME SIZE
        // This sets all the colors of the pixles to a defualt state. This sets the alpha is to 0, this allows texture to be seethough if needed.
        Color[] clearPixels = new Color[mapTexture.width * mapTexture.height];
        mapTexture.SetPixels(clearPixels);

        int x, y;
        for (x = 0; x < width; x++)
        {
            for (y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    // Get the colors
                    Color[] pix = variant[Random.Range(0, variant.Length)].GetPixels(0, 0, 32, 32);/// CHANGE TEXTURE2D PIXEL SIZE HERE
                    // Set the colors at a positon in the texture 
                    // multiplying by 32 assumes all the textures are square.
                    mapTexture.SetPixels(32 * x, 32 * y, 32, 32, pix);/// CHANGE TEXTURE2D PIXEL SIZE HERE
                }
            }
        }
        //
        mapTexture.wrapMode = TextureWrapMode.Clamp;
        mapTexture.filterMode = FilterMode.Point;
        //Apply settings
        mapTexture.Apply();
        // set the texture
        theTextureGO.GetComponent<SpriteRenderer>().sprite = MakeSprite(mapTexture);
        // adjust the position if needed
        theTextureGO.transform.position = new Vector3(0.0f, depth, 0.0f); 
        // adjust the size if needed
        theTextureGO.transform.localScale = new Vector3(3.0f, 3.0f);
    }
    // Create a sprite from a texture
    public Sprite MakeSprite(Texture2D texture)
    { 
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }
}
