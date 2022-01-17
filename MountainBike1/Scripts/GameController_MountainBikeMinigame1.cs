using DG.Tweening;
using GestureRecognizer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController_MountainBikeMinigame1 : MonoBehaviour
{
    public static GameController_MountainBikeMinigame1 instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(instance);
    }

    public MountainBike_MountainBikeMinigame1 mountainBike;
    public Camera mainCamera;
    public bool isFollow;
    public float offsetY;
    public float posYCameraFollow;
    public Recognizer recognizer;
    public LinerObject_MountainBikeMinigame1 dreamPrefab;
    public List<LinerObject_MountainBikeMinigame1> dreamObj;
    public Canvas canvas;
    public Transform referenceRoot;
    public DrawDetector drawDetector;
    public Color32 colors;
    public List<Transform> dreamPos;
    public int stage, num, directionY;
    public bool isLock = false;
    public bool isFirst = true;
    public GameObject imagePrefab;
    public GameObject image;
    public Vector2 preMousePos, currentMousePos;
    public List<int> patternIndexs = new List<int>();
    public GameObject tutorial;

    private void Start()
    {
        tutorial.SetActive(false);
        isFollow = false;
        directionY = 0;
        num = 0;
        stage = 1;
        mainCamera.DOOrthoSize(5, 2f);
        mainCamera.transform.DOMove(new Vector3(mountainBike.transform.position.x, -2.01f, mainCamera.transform.position.z), 2f).OnComplete(() =>
        {
            isFollow = true;
            offsetY = mountainBike.transform.position.y - mainCamera.transform.position.y;
        });

        SetSizeCamera();
    }

    void SetSizeCamera()
    {
        float f1 = 16.0f / 9;
        float f2 = Screen.width * 1.0f / Screen.height;

        mainCamera.orthographicSize *= f1 / f2;
    }

    LinerObject_MountainBikeMinigame1 SpawnDream(int dreamIndex)
    {
        LinerObject_MountainBikeMinigame1 dream = Instantiate(dreamPrefab);
        dream.transform.parent = referenceRoot.transform;
        dream.transform.localScale = Vector3.one;

        if (dreamIndex == 1)
        {
            dream.line.RemoveAt(1);
            dream.line.RemoveAt(1);
        }
        if (dreamIndex == 2)
        {
            dream.line.RemoveAt(2);
        }

        for (int i = 0; i < dreamIndex; i++)
        {
            dream.transform.GetChild(0).GetChild(i).GetComponent<GesturePatternDraw>().pattern = recognizer.patterns[patternIndexs[i]];
            dream.transform.GetChild(0).GetChild(i).GetComponent<GesturePatternDraw>().color = colors;
            dream.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
        }
        dream.GetComponent<RectTransform>().localPosition = Vector3.zero;

        image = Instantiate(imagePrefab.transform.GetChild(stage - 1).gameObject, dreamPos[stage - 1]);

        return dream;
    }

    public Vector2 ConvertWorldPossitionToCanvasPossition(Vector2 worldPos)
    {
        RectTransform CanvasRect = canvas.GetComponent<RectTransform>();
        Vector2 ViewportPosition = mainCamera.WorldToViewportPoint(worldPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
        return WorldObject_ScreenPosition;
    }

    public void AddStage()
    {
        if (stage == 1)
        {
            patternIndexs.Clear();
            patternIndexs.Add(1);
            dreamObj.Add(SpawnDream(1));
        }
        if (stage == 2)
        {
            patternIndexs.Clear();
            patternIndexs.Add(0);
            patternIndexs.Add(1);
            directionY = 1;
            dreamObj.Add(SpawnDream(2));
        }
        if (stage == 3)
        {
            patternIndexs.Clear();
            patternIndexs.Add(2);
            dreamObj.Add(SpawnDream(1));
        }
        if (stage == 4)
        {
            patternIndexs.Clear();
            patternIndexs.Add(4);
            patternIndexs.Add(0);
            directionY = 2;
            dreamObj.Add(SpawnDream(2));
        }
        if (stage == 5)
        {
            patternIndexs.Clear();
            patternIndexs.Add(0);
            patternIndexs.Add(1);
            directionY = 1;
            dreamObj.Add(SpawnDream(2));
        }
        if (stage == 6)
        {
            patternIndexs.Clear();
            patternIndexs.Add(2);
            dreamObj.Add(SpawnDream(1));
        }
        if (stage == 7)
        {
            patternIndexs.Clear();
            patternIndexs.Add(1);
            patternIndexs.Add(1);
            dreamObj.Add(SpawnDream(2));
        }
        if (stage == 8)
        {
            patternIndexs.Clear();
            patternIndexs.Add(1);
            patternIndexs.Add(2);
            patternIndexs.Add(1);
            dreamObj.Add(SpawnDream(3));
        }
        if (stage == 9)
        {
            patternIndexs.Clear();
            patternIndexs.Add(3);
            dreamObj.Add(SpawnDream(1));
        }
    }

    public void OnRecognize(RecognitionResult result)
    {
        if (result != RecognitionResult.Empty)
        {
            Blink(result.gesture.id);
        }
    }

    void Blink(string id)
    {
        for (int i = 0; i < dreamObj.Count; i++)
        {
            int j = 0;
            while (!dreamObj[i].line[j].IsActive())
            {
                j++;
            }
            if (dreamObj[i].line[j].pattern.id == id)
            {
                if (id == "vertical")
                {
                    if (directionY == 1)
                    {
                        if (currentMousePos.y - preMousePos.y > 0)
                        {
                            DrawSuccessOneDream(dreamObj[i]);
                        }
                    }
                    if (directionY == 2)
                    {
                        if (preMousePos.y - currentMousePos.y > 0)
                        {
                            DrawSuccessOneDream(dreamObj[i]);
                        }
                    }
                }
                else
                {
                    if (currentMousePos.x - preMousePos.x > 0)
                    {
                        DrawSuccessOneDream(dreamObj[i]);
                    }
                }
            }
        }
    }
    void DrawSuccessOneDream(LinerObject_MountainBikeMinigame1 dream)
    {
        dream.Sleep();
        if (dream.line.Count == 0)
        {
            image.transform.GetChild(num).GetComponent<SpriteRenderer>().DOFade(0, 0.5f);
            MountainBike_MountainBikeMinigame1.instance.jump = true;
            UpdateStage(dream);
        }
        else
        {
            image.transform.GetChild(num).GetComponent<SpriteRenderer>().DOFade(0, 0.025f);
            num++;
        }
    }

    void UpdateStage(LinerObject_MountainBikeMinigame1 dream)
    {
        if (isFirst)
        {
            isFirst = false;
            tutorial.SetActive(false);
            Destroy(tutorial);
        }
        MountainBike_MountainBikeMinigame1.instance.timeManager.isSlow = false;
        MountainBike_MountainBikeMinigame1.instance.isPass = true;
        Destroy(image, 1f);
        dreamObj.Remove(dream);
        dream.gameObject.SetActive(false);
        Destroy(dream.gameObject, 0.1f);
        stage++;
        isLock = false;
        num = 0;
    }

    public void LoseGame()
    {
        if (tutorial)
        {
            Destroy(tutorial);
        }
        isFollow = false;
        drawDetector.gameObject.SetActive(false);
        Destroy(mountainBike.gameObject, 5f);
    }

    public void WinGame()
    {
        isFollow = false;
        drawDetector.gameObject.SetActive(false);
        mountainBike.transform.DOMoveX(mountainBike.transform.position.x + 20, 3f);
        Destroy(mountainBike.gameObject, 5f);
    }

    private void Update()
    {
        if (isFollow)
        {
            posYCameraFollow = mountainBike.transform.position.y - offsetY;
            if (posYCameraFollow <= -2.01f)
            {
                posYCameraFollow = -2.01f;
            }
            mainCamera.transform.position = new Vector3(mountainBike.transform.position.x, posYCameraFollow, mainCamera.transform.position.z);

            if (Input.GetMouseButtonDown(0))
            {
                preMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                currentMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
        }


    }
}

