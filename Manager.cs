using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public GameObject runnerSprite;
    public GameObject sensorSprite;

    private bool isTraining = false;
    private int populationSize = 52;
    private int generationNumber = 0;
    private float maxFitness;
    private float prevFitness = 0;
    private int[] layers = new int[] { 3, 6, 6, 2 };
    private List<NeuralNetwork> nets;
    private List<Runner> runList = null;
    private int fittestIndex = 0;
    private GameObject headPointer;
    private GameObject leftPointer;
    private GameObject rightPointer;

    void Timer()
    {
        isTraining = false;
    }

    void Update ()
    {
        if (isTraining == false)
        {
            if (generationNumber == 0)
            {
                InitCarNeuralNetworks();
            }
            else
            {
                nets.Sort();
                maxFitness = nets[populationSize - 1].GetFitness();
                for (int i = 0; i < populationSize - 2; i++)
                {
                    nets[i].Crossover(nets[populationSize - 1], nets[populationSize - 2], 1/(maxFitness - prevFitness));
                    nets[i].SetFitness(0f);
                }
                prevFitness = maxFitness;
            }
            generationNumber++;
            isTraining = true;
            Invoke("Timer", 10f);
            Destroy(headPointer);
            Destroy(leftPointer);
            Destroy(rightPointer);
            CreateRunnerBodies();
        }
        else
        {
            for (int i = 0; i < populationSize; i++)
            {
                runList[fittestIndex].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                if (nets[i].GetFitness() > nets[fittestIndex].GetFitness())
                {
                    fittestIndex = i;
                }
            }
            runList[fittestIndex].GetComponent<SpriteRenderer>().color = new Color (0f, 0.65f, 0.2f);
            headPointer.transform.position = runList[fittestIndex].transform.position + (0.4f + runList[fittestIndex].headDistance) * runList[fittestIndex].transform.up;           
            headPointer.transform.rotation = runList[fittestIndex].transform.rotation;
            leftPointer.transform.position = runList[fittestIndex].transform.position + Quaternion.AngleAxis(60, transform.forward) * runList[fittestIndex].transform.up * (0.15f + runList[fittestIndex].leftDistance);
            leftPointer.transform.rotation = Quaternion.AngleAxis(60, transform.forward) * runList[fittestIndex].transform.rotation;
            rightPointer.transform.position = runList[fittestIndex].transform.position + Quaternion.AngleAxis(-60, transform.forward) * runList[fittestIndex].transform.up * (0.15f + runList[fittestIndex].rightDistance);
            rightPointer.transform.rotation = Quaternion.AngleAxis(-60, transform.forward) * runList[fittestIndex].transform.rotation;
        }
    }
    private void CreateRunnerBodies()
    {
        if (runList != null)
        {
            for (int i = 0; i < runList.Count; i++)
            {
                GameObject.Destroy(runList[i].gameObject);
            }
        }
        runList = new List<Runner>();

        for (int i = 0; i < populationSize; i++)
        {
            Runner run = ((GameObject)Instantiate(runnerSprite, Vector2.zero, runnerSprite.transform.rotation)).GetComponent<Runner>();
            run.Init(nets[i]);
            runList.Add(run);
        }
        headPointer = Instantiate(sensorSprite);
        leftPointer = Instantiate(sensorSprite);
        rightPointer = Instantiate(sensorSprite);
    }
    void InitCarNeuralNetworks()
    {
        nets = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Randomise();
            nets.Add(net);
        }
    }
}
