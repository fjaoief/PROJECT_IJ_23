using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
 
public class tilemap_lr : MonoBehaviour
{
    public GameObject player;

    private Vector3Int previous;
    int mapsize;
    public int interval = 50; 

 
    [SerializeField]
    private Tilemap tilemap;

    void Start () {
        tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
        previous = tilemap.WorldToCell(player.transform.position);
        mapsize = tilemap.cellBounds.size.x / 2;
      
    } 


    // do late so that the player has a chance to move in update if necessary
    private void LateUpdate()
    {
        tilemap = GetComponent<Tilemap>();
        Vector3Int currentCell = tilemap.WorldToCell(player.transform.position);

        if (currentCell.x - previous.x <= interval * -1 || currentCell.x - previous.x >=interval){
            BoundsInt bounds = tilemap.cellBounds;
            int location = currentCell.x;

            // 왼쪽으로 interval 이상 갔을 때
            if(location - previous.x <= interval * -1){
                for (int x = bounds.xMax-interval; x < bounds.xMax; x++) {
                    for (int y = bounds.yMin; y < bounds.yMax; y++) {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                        tilemap.SetTile(new Vector3Int(x, y, 0), null);
                        tilemap.SetTile(new Vector3Int(x - mapsize * 2, y, 0), tile);
                    }
                }   
            } 
            // 오른쪽으로 interval 이상 갔을 때
            else{
                for (int x = bounds.xMin; x < bounds.xMin + interval; x++) {
                    for (int y = bounds.yMin; y < bounds.yMax; y++) {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                        tilemap.SetTile(new Vector3Int(x, y, 0), null);
                        tilemap.SetTile(new Vector3Int(x + mapsize*2, y, 0), tile);
                    }
                }  
            }

            tilemap.CompressBounds();
            previous = currentCell;
        }
    } 
}