using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ST_PuzzleDisplay_diva : MonoBehaviour
{
    // 퍼즐 Prefabs
    public GameObject puzzle;

    // UI 캔버스
    public GameObject g_Canvas;
    public GameObject p_Canvas;

    // 카메라
    public Camera playerCam;
    public Camera puzzleCam;
    public Camera aroundCam;

    // 버튼
    public Button completeButton;
    public Button gobackButton;

    // 퍼즐 이미지
    public Texture PuzzleImage;

    // 퍼즐의 가로 새로 개수 설정
    public int Height = 3;
    public int Width = 3;

    // 퍼즐 크기 값
    public Vector3 PuzzleScale = new Vector3(1.0f, 1.0f, 1.0f);

    // 퍼즐 위치 값
    public Vector3 PuzzlePosition = new Vector3(0.0f, 0.0f, 0.0f);

    // 퍼즐 사이의 거리 지정
    public float SeperationBetweenTiles = 0.5f;

    // 퍼즐 조각 오브젝트
    public GameObject Tile;

    // 퍼즐 랜더에 사용할 쉐이더 설정
    public Shader PuzzleShader;

    // 퍼즐조각을 배열로 설정
    private GameObject[,] TileDisplayArray;
    private List<Vector3> DisplayPositions = new List<Vector3>();

    // 마지막 조각 생성
    private GameObject LastDisplayTile;

    // 위치와 크기 지정 변수
    private Vector3 Scale;
    private Vector3 Position;

    // 퍼즐이 완성되었는지 설정
    public bool Complete = false;

    // 현재 실행되고 있는지 확인
    private bool check = false;

    private void Start()
    {
        // 제공된 이미지로 퍼즐 생성
        CreatePuzzleTiles();
    }

    private void Update()
    {
        if (csStartPuzzle_diva.checkStart == true)
        {
            GetComponent<AudioSource>().Play();
            check = true;
            csStartPuzzle_diva.checkStart = false;
            makePuzzle();
        }

        // 퍼즐이 움직인 위치를 재설정
        this.transform.localPosition = PuzzlePosition;

        // 전체 퍼즐 오브젝트의 크기를 재설정
        this.transform.localScale = PuzzleScale;
    }

    public Vector3 GetTargetLocation(ST_PuzzleTile_diva thisTile)
    {
        // 움직일 수 있는지 확인하고 목적지 정보 확인
        ST_PuzzleTile_diva MoveTo = CheckIfWeCanMove((int)thisTile.GridLocation.x, (int)thisTile.GridLocation.y, thisTile);

        if (MoveTo != thisTile)
        {
            // 새로운 타일의 목적지 정보를 저장 
            Vector3 TargetPos = MoveTo.TargetPosition;
            Vector2 GridLocation = thisTile.GridLocation;
            thisTile.GridLocation = MoveTo.GridLocation;

            // 빈 타일의 위치로 옮김
            MoveTo.LaunchPositionCoroutine(thisTile.TargetPosition);
            MoveTo.GridLocation = GridLocation;

            // 새로운 위치 정보 반환
            return TargetPos;
        }

        // 현재 위치 그대로 반환
        return thisTile.TargetPosition;
    }

    private ST_PuzzleTile_diva CheckMoveLeft(int Xpos, int Ypos, ST_PuzzleTile_diva thisTile)
    {
        // 왼쪽 체크
        if ((Xpos - 1) >= 0)
        {
            // 왼쪽으로 움직일수 있으면
            return GetTileAtThisGridLocation(Xpos - 1, Ypos, thisTile);
        }

        return thisTile;
    }

    private ST_PuzzleTile_diva CheckMoveRight(int Xpos, int Ypos, ST_PuzzleTile_diva thisTile)
    {
        if ((Xpos + 1) < Width)
        {
            return GetTileAtThisGridLocation(Xpos + 1, Ypos, thisTile);
        }

        return thisTile;
    }

    private ST_PuzzleTile_diva CheckMoveDown(int Xpos, int Ypos, ST_PuzzleTile_diva thisTile)
    {
        if ((Ypos - 1) >= 0)
        {
            return GetTileAtThisGridLocation(Xpos, Ypos - 1, thisTile);
        }

        return thisTile;
    }

    private ST_PuzzleTile_diva CheckMoveUp(int Xpos, int Ypos, ST_PuzzleTile_diva thisTile)
    {
        if ((Ypos + 1) < Height)
        {
            return GetTileAtThisGridLocation(Xpos, Ypos + 1, thisTile);
        }

        return thisTile;
    }

    private ST_PuzzleTile_diva CheckIfWeCanMove(int Xpos, int Ypos, ST_PuzzleTile_diva thisTile)
    {
        // 움직일 방향 설정
        if (CheckMoveLeft(Xpos, Ypos, thisTile) != thisTile)
        {
            return CheckMoveLeft(Xpos, Ypos, thisTile);
        }

        if (CheckMoveRight(Xpos, Ypos, thisTile) != thisTile)
        {
            return CheckMoveRight(Xpos, Ypos, thisTile);
        }

        if (CheckMoveDown(Xpos, Ypos, thisTile) != thisTile)
        {
            return CheckMoveDown(Xpos, Ypos, thisTile);
        }

        if (CheckMoveUp(Xpos, Ypos, thisTile) != thisTile)
        {
            return CheckMoveUp(Xpos, Ypos, thisTile);
        }

        return thisTile;
    }

    private ST_PuzzleTile_diva GetTileAtThisGridLocation(int x, int y, ST_PuzzleTile_diva thisTile)
    {
        // 퍼즐의 위치가 맞게 들어갔는지 왼쪽 상단부터 확인
        for (int j = Height - 1; j >= 0; j--)
        {
            for (int i = 0; i < Width; i++)
            {

                if ((TileDisplayArray[i, j].GetComponent<ST_PuzzleTile_diva>().GridLocation.x == x) &&
                   (TileDisplayArray[i, j].GetComponent<ST_PuzzleTile_diva>().GridLocation.y == y))
                {
                    // 비활성화 시킨 타일을 발견하면 해당 타일을 반환
                    if (TileDisplayArray[i, j].GetComponent<ST_PuzzleTile_diva>().Active == false)
                    {
                        return TileDisplayArray[i, j].GetComponent<ST_PuzzleTile_diva>();
                    }
                }
            }
        }

        return thisTile;
    }

    private IEnumerator JugglePuzzle()
    {
        yield return new WaitForSeconds(1.0f);

        // 왼쪽 하단의 퍼즐을 비활성화
        // { (0,2) (1,2) (2,2)
        //   (0,1) (1,1) (2,1)
        //   (0,0) (1,0) (2,0) }
        TileDisplayArray[0, 0].GetComponent<ST_PuzzleTile_diva>().Active = false;

        yield return new WaitForSeconds(1.0f);

        for (int k = 0; k < 2; k++)
        {
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    TileDisplayArray[i, j].GetComponent<ST_PuzzleTile_diva>().ExecuteAdditionalMove();

                    yield return new WaitForSeconds(0.02f);
                }
            }
        }

        // 퍼즐이 완료되었는지 확인
        StartCoroutine(CheckForComplete());

        yield return null;
    }

    public IEnumerator CheckForComplete()
    {
        while (Complete == false)
        {
            // 모든 타일이 올바른 위치에 있는지 확인
            Complete = true;
            for (int j = Height - 1; j >= 0; j--)
            {
                for (int i = 0; i < Width; i++)
                {
                    // 올바른 위치에 없다면 false로 만듬
                    if (TileDisplayArray[i, j].GetComponent<ST_PuzzleTile_diva>().CorrectLocation == false)
                    {
                        Complete = false;
                    }
                }
            }

            yield return null;
        }

        if (Complete)
        {
            CompletePuzzleTiles();
            completeButton.gameObject.SetActive(true);
            gobackButton.gameObject.SetActive(false);
        }
        yield return null;
    }

    private Vector2 ConvertIndexToGrid(int index)
    {
        int WidthIndex = index;
        int HeightIndex = 0;

        // 퍼즐의 배열정보 반환
        for (int i = 0; i < Height; i++)
        {
            if (WidthIndex < Width)
            {
                return new Vector2(WidthIndex, HeightIndex);
            }
            else
            {
                WidthIndex -= Width;
                HeightIndex++;
            }
        }

        return new Vector2(WidthIndex, HeightIndex);
    }

    private void CreatePuzzleTiles()
    {
        // 배열 생성
        TileDisplayArray = new GameObject[Width, Height];

        // 크기와 위치 설정
        Scale = new Vector3(1.0f / Width, 1.0f, 1.0f / Height);
        Tile.transform.localScale = Scale;

        // 타일에 번호값 설정
        int TileValue = 0;

        // 타일 생성
        for (int j = Height - 1; j >= 0; j--)
        {
            for (int i = 0; i < Width; i++)
            {
                // 위치 설정
                Position = new Vector3(((Scale.x * (i + 0.5f)) - (Scale.x * (Width / 2.0f))) * (10.0f + SeperationBetweenTiles),
                                       0.0f,
                                      ((Scale.z * (j + 0.5f)) - (Scale.z * (Height / 2.0f))) * (10.0f + SeperationBetweenTiles));

                // 위치 설정
                DisplayPositions.Add(Position);

                // 타일 생성
                TileDisplayArray[i, j] = Instantiate(Tile, PuzzlePosition, Quaternion.Euler(90.0f, -180.0f, 0.0f)) as GameObject;
                TileDisplayArray[i, j].gameObject.transform.parent = this.transform;

                // 카운드 세팅과 증가
                ST_PuzzleTile_diva thisTile = TileDisplayArray[i, j].GetComponent<ST_PuzzleTile_diva>();
                thisTile.ArrayLocation = new Vector2(i, j);
                thisTile.GridLocation = new Vector2(i, j);
                thisTile.LaunchPositionCoroutine(Position);
                TileValue++;

                // Material 생성
                Material thisTileMaterial = new Material(PuzzleShader);

                // 퍼즐 이미지 저장
                thisTileMaterial.mainTexture = PuzzleImage;

                // material값 저장
                thisTileMaterial.mainTextureOffset = new Vector2(1.0f / Width * i, 1.0f / Height * j);
                thisTileMaterial.mainTextureScale = new Vector2(1.0f / Width, 1.0f / Height);

                // 해당 타일을 위한 새로운 material 생성
                TileDisplayArray[i, j].GetComponent<Renderer>().material = thisTileMaterial;
            }
        }
    }

    private void CompletePuzzleTiles()
    {
        // 크기 저장
        Scale = new Vector3(1.0f / Width, 1.0f, 1.0f / Height);
        Tile.transform.localScale = Scale;


        // 위치 설정
        Position = new Vector3(((Scale.x * 0.5f) - (Scale.x * (Width / 2.0f))) * (10.0f + SeperationBetweenTiles),
                               0.0f,
                              ((Scale.z * 0.5f) - (Scale.z * (Height / 2.0f))) * (10.0f + SeperationBetweenTiles));

        // 위치 설정
        DisplayPositions.Add(Position);

        // 타일 생성
        LastDisplayTile = Instantiate(Tile, gameObject.transform.position, Quaternion.Euler(90.0f, -180.0f, 0.0f)) as GameObject;
        LastDisplayTile.gameObject.transform.parent = this.transform;

        // 타일 세팅
        ST_PuzzleTile_diva thisTile = LastDisplayTile.GetComponent<ST_PuzzleTile_diva>();
        thisTile.ArrayLocation = new Vector2(0, 0);
        thisTile.GridLocation = new Vector2(0, 0);
        thisTile.LaunchPositionCoroutine(Position);

        // Material 생성
        Material thisTileMaterial = new Material(PuzzleShader);

        // 퍼즐 이미지 저장
        thisTileMaterial.mainTexture = PuzzleImage;

        // material값 저장
        thisTileMaterial.mainTextureOffset = new Vector2(0, 0);
        thisTileMaterial.mainTextureScale = new Vector2(1.0f / Width, 1.0f / Height);

        // 해당 타일을 위한 새로운 material 생성
        LastDisplayTile.GetComponent<Renderer>().material = thisTileMaterial;
    }

    private void makePuzzle()
    {
        // 퍼즐 섞기
        StartCoroutine(JugglePuzzle());

        // UI 초기화
        completeButton.gameObject.SetActive(false);
        gobackButton.gameObject.SetActive(true);
    }

    public void completebutton()
    {
        if (check)
        {
            GetComponent<AudioSource>().Stop();

            playerCam.gameObject.SetActive(true);
            puzzleCam.gameObject.SetActive(false);
            aroundCam.gameObject.SetActive(false);

            g_Canvas.SetActive(true);
            p_Canvas.SetActive(false);

            check = false;
            csStartPuzzle_diva.checkComplete = true;
            Destroy(puzzle);
        }
    }

    public void gobackbutton()
    {
        if (check)
        {
            GetComponent<AudioSource>().Stop();

            playerCam.gameObject.SetActive(true);
            puzzleCam.gameObject.SetActive(false);
            aroundCam.gameObject.SetActive(false);

            g_Canvas.SetActive(true);
            p_Canvas.SetActive(false);

            csStartPuzzle_diva.checkGoback = true;
            check = false;
        }
    }
}
