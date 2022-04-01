using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BottleController : MonoBehaviour
{

    public List<Color> bottleColors;
    public SpriteRenderer bottleMaskSR;

    public AnimationCurve scaleRotationMC;
    public AnimationCurve fillAmountC;
    public AnimationCurve RotattionSpeedMultiplier;



    public List<float> fillAmounts;
    public List<float> rotationsValues;

    private int rotationIndex = 0;

    [Range(0, 4)]
    public int numberOfColorInBottle = 4;


    public Color topColor;
    public int numberOfTopColorLayers = 1;

    public BottleController bottleControlRef;
    public bool justThisBottle = false;
    private int numberOfColorTransfer = 0;

    public Transform leftRotatePoint;
    public Transform rightRotatePoint;
    public Transform chosenRotatePoint;


    private float directionMultiplier = 1f;


    Vector3 originPos;
    Vector3 startPos;
    Vector3 endPos;


    public LineRenderer lineRenderer;



    [SerializeField] MenuController menuController;

    [SerializeField] GameObject fullBottleEffect,finishEffect;
    void Start()
    {

        menuController = GameObject.Find("GameController").GetComponent<MenuController>();
        bottleMaskSR.material.SetFloat("_FillAmount", fillAmounts[numberOfColorInBottle]);

        originPos = transform.position;

        UpdateColorsOnShaders();
        UpdateTopColorValues();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyUp(KeyCode.P) && justThisBottle == true)
        {

            UpdateTopColorValues();

            if (bottleControlRef.FillBottleCheck(topColor))
            {

                ChoseRotationPointAndDirection();
                numberOfColorTransfer = Mathf.Min(numberOfTopColorLayers, 4 - bottleControlRef.numberOfColorInBottle);


                for (int i = 0; i < numberOfColorTransfer; i++)
                {
                    bottleControlRef.bottleColors[bottleControlRef.numberOfColorInBottle + i] = topColor;
                }

                bottleControlRef.UpdateColorsOnShaders();
            }


            bottleControlRef.UpdateColorsOnShaders();

            CalculateRotationIndex(4 - bottleControlRef.numberOfColorInBottle);
            StartCoroutine(RotateBottle());
        }
    }

    public void StartColorTransfer()
    {
        ChoseRotationPointAndDirection();
        numberOfColorTransfer = Mathf.Min(numberOfTopColorLayers, 4 - bottleControlRef.numberOfColorInBottle);


        for (int i = 0; i < numberOfColorTransfer; i++)
        {
            bottleControlRef.bottleColors[bottleControlRef.numberOfColorInBottle + i] = topColor;
        }

        bottleControlRef.UpdateColorsOnShaders();

        CalculateRotationIndex(4 - bottleControlRef.numberOfColorInBottle);
        DisableBottle();
        StartCoroutine(MoveBottle());

    }

    private void DisableBottle()
    {
        GameObject[] bottles = GameObject.FindGameObjectsWithTag("bottle");
        for (int i = 0; i < bottles.Length; i++)
        {
            bottles[i].GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    IEnumerator MoveBottle()
    {
        startPos = transform.position;
        if (chosenRotatePoint == leftRotatePoint)
        {
            endPos = bottleControlRef.rightRotatePoint.position;
        }
        else
        {
            endPos = bottleControlRef.leftRotatePoint.position;
        }
        float t = 0;

        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime * 3;

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;

        StartCoroutine(RotateBottle());
    }

    IEnumerator MoveBottleBack()
    {
        startPos = transform.position;
        endPos = originPos;

        float t = 0;

        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime * 2;

            yield return new WaitForEndOfFrame();
        }
        transform.position = endPos;

        ControlBottle();
    }

    private void ControlBottle()
    {
        GameObject[] bottles = GameObject.FindGameObjectsWithTag("bottle");
        int count = 0;
        for (int i = 0; i < bottles.Length; i++)
        {
            if (bottles[i].GetComponent<BottleController>().numberOfColorInBottle == 4)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (bottles[i].GetComponent<BottleController>().bottleColors[0] != bottles[i].GetComponent<BottleController>().bottleColors[x])
                    {
                        count = 0;
                        break;
                    }
                    else count++;
                }

                if (count > 3)
                {
                    Instantiate(fullBottleEffect, bottles[i].transform.position, Quaternion.identity);
                    bottles[i].tag = "finish";
                    bottles[i].GetComponent<BoxCollider2D>().enabled = false;
                    LevelController.levelWinPoint--;

                    if (0 == LevelController.levelWinPoint)
                    {
                        StartCoroutine(WaitWinEffect());
                    }
                }
            }
        }

        bottles = GameObject.FindGameObjectsWithTag("bottle");
        for (int i = 0; i < bottles.Length; i++)
        {
            bottles[i].GetComponent<BoxCollider2D>().enabled = true;
        }
    }


    IEnumerator WaitWinEffect()
    {
        Instantiate(finishEffect, new Vector3(0, 0, 0), Quaternion.identity);       
        menuController.WinPanel();
        yield return new WaitForSeconds(1.5f);
    }
    void UpdateColorsOnShaders()
    {
        bottleMaskSR.material.SetColor("_C1", bottleColors[0]);
        bottleMaskSR.material.SetColor("_C2", bottleColors[1]);
        bottleMaskSR.material.SetColor("_C3", bottleColors[2]);
        bottleMaskSR.material.SetColor("_C4", bottleColors[3]);
    }

    public float timeToRotate = 1.0f;

    IEnumerator RotateBottle()
    {
        float t = 0;
        float lerpValue;
        float angleValue;
        float lastAngleValue = 0;

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(0.0f, directionMultiplier * rotationsValues[rotationIndex], lerpValue);
            // transform.eulerAngles = new Vector3(0, 0, angleValue);

            transform.RotateAround(chosenRotatePoint.position, Vector3.forward, lastAngleValue - angleValue);

            bottleMaskSR.material.SetFloat("_SARM", scaleRotationMC.Evaluate(angleValue));


            if (fillAmounts[numberOfColorInBottle] > fillAmountC.Evaluate(angleValue) + 0.005f)
            {

                if (lineRenderer.enabled == false && fillAmountC.Evaluate(angleValue) < 0.63f)
                {
                    lineRenderer.startColor = topColor;
                    lineRenderer.endColor = topColor;
                    lineRenderer.SetPosition(0, chosenRotatePoint.position);
                    lineRenderer.SetPosition(1, chosenRotatePoint.position - Vector3.up * 1.45f);
                    lineRenderer.enabled = true;
                }

                bottleMaskSR.material.SetFloat("_FillAmount", fillAmountC.Evaluate(angleValue));
                bottleControlRef.FillUp(fillAmountC.Evaluate(lastAngleValue) - fillAmountC.Evaluate(angleValue));
            }


            t += Time.deltaTime * RotattionSpeedMultiplier.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();
        }

        angleValue = directionMultiplier * rotationsValues[rotationIndex];
        //  transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_SARM", scaleRotationMC.Evaluate(angleValue));
        bottleMaskSR.material.SetFloat("_FillAmount", fillAmountC.Evaluate(angleValue));

        numberOfColorInBottle -= numberOfColorTransfer;
        bottleControlRef.numberOfColorInBottle += numberOfColorTransfer;

        lineRenderer.enabled = false;
        StartCoroutine(RotateBottleBack());
    }


    IEnumerator RotateBottleBack()
    {

        float t = 0;
        float lerpValue;
        float angleValue;

        float lastAngelValue = directionMultiplier * rotationsValues[rotationIndex];

        while (t < timeToRotate)
        {
            lerpValue = t / timeToRotate;
            angleValue = Mathf.Lerp(directionMultiplier * rotationsValues[rotationIndex], 0.0f, lerpValue);
            //transform.eulerAngles = new Vector3(0, 0, angleValue);

            transform.RotateAround(chosenRotatePoint.position, Vector3.forward, lastAngelValue - angleValue);

            bottleMaskSR.material.SetFloat("_SARM", scaleRotationMC.Evaluate(angleValue));

            lastAngelValue = angleValue;

            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        UpdateTopColorValues();
        angleValue = 0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_SARM", scaleRotationMC.Evaluate(angleValue));

        StartCoroutine(MoveBottleBack());
    }


    public void UpdateTopColorValues()
    {
        if (numberOfColorInBottle != 0)
        {
            numberOfTopColorLayers = 1;
            topColor = bottleColors[numberOfColorInBottle - 1];

            if (numberOfColorInBottle == 4)
            {
                if (bottleColors[3].Equals(bottleColors[2]))
                {
                    numberOfTopColorLayers = 2;
                    if (bottleColors[2].Equals(bottleColors[1]))
                    {
                        numberOfTopColorLayers = 3;

                        if (bottleColors[1].Equals(bottleColors[0]))
                        {
                            numberOfTopColorLayers = 4;
                        }
                    }
                }

            }

            else if (numberOfColorInBottle == 3)
            {
                if (bottleColors[2].Equals(bottleColors[1]))
                {
                    numberOfTopColorLayers = 2;
                    if (bottleColors[1].Equals(bottleColors[0]))
                    {
                        numberOfTopColorLayers = 3;

                    }
                }
            }

            else if (numberOfColorInBottle == 2)
            {
                if (bottleColors[1].Equals(bottleColors[0]))
                {
                    numberOfTopColorLayers = 2;
                }
            }

            rotationIndex = 3 - (numberOfColorInBottle - numberOfTopColorLayers);
        }

    }

    public bool FillBottleCheck(Color colorToCheck)
    {
        if (numberOfColorInBottle == 0) return true;
        else
        {
            if (numberOfColorInBottle == 4) return false;
            else
            {
                if (topColor.Equals(colorToCheck)) return true;
                else return false;
            }
        }

    }



    private void CalculateRotationIndex(int nextBottle)
    {
        rotationIndex = 3 - (numberOfColorInBottle - Mathf.Min(nextBottle, numberOfTopColorLayers));

    }


    private void FillUp(float fillAmountToAdd)
    {
        bottleMaskSR.material.SetFloat("_FillAmount", bottleMaskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
    }


    private void ChoseRotationPointAndDirection()
    {
        if (transform.position.x > bottleControlRef.transform.position.x)
        {
            chosenRotatePoint = leftRotatePoint;
            directionMultiplier = -1.0f;
        }
        else
        {
            chosenRotatePoint = rightRotatePoint;
            directionMultiplier = 1.0f;
        }
    }






}
