using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.IO;
using System;



public class Route : MonoBehaviour
{
    public delegate void DrawRouteType();
    public delegate float CalcRouteType(Vector3 p1, Vector3 p2, float f);
    public delegate CalcRouteType GetRouteType();

    public delegate float CalcMinPointType();
    public delegate float CalcScoreType(float f);
    public delegate bool CalcIndex();

    public class RouteInfo {

        public string name;
        public CalcRouteType calc;
        public DrawRouteType draw;
        public CalcMinPointType calcMinDis;


        public RouteInfo(string _name, CalcRouteType _calc, DrawRouteType _draw, CalcMinPointType _calcMinDis)
        {
            name = _name;
            calc = _calc;
            draw = _draw;
            calcMinDis = _calcMinDis;
        }

    };
    public class Data 
    {
        public float errorAvg;
        public float time;
        
        public Data(float _errorAvg, float _time) 
        {
            time = _time;
            errorAvg = _errorAvg;
        }
    };

    public List<RouteInfo> RouteInfoArray;
    public List<CalcScoreType> CalcScoreArray;
    

    public GameObject Player;
    public GameObject Goal;
    public GameObject UI_dropdown;
    public GameObject SineWave;
    public GameObject ErrorValue;
    public GameObject time;
    public Text errorText;
    public Text TimeText;
    public Text timerText;
    public Text trialText; 
    /* the base point for drawing Line */
    private Vector3 p1, p2;

    private const int up = 1, down = 0;

    /* the base for circle*/
    private Vector3 middle_point;
    private float r;

    /* Linerenderer */
    private LineRenderer lineRenderer;

    /* Version 2*/
    private int passIndex;
    private Vector3 cur;
    private Vector3 next;
    private bool[] isPassed;
    private float errorAvg;
    private int frameCount;
    private float currentTime;
    private float waitTime;
    private int trial;
    public bool isWait;
    public int maxTrial;
    private List<Data> errorData;
    private float currentError;
    public int testMode;

    private int routeNum;
    private int calcNum;
    private int frequencyIndex;
    public List<CalcIndex> tryNextIndex;
    private int[] frequencyList;

    private string fileName;
    StreamWriter sw;

    /* Debugging */
    string TEST;


    // Start is called before the first frame update
    void Start()
    {

        RouteInfoArray = new List<RouteInfo>();
        CalcScoreArray = new List<CalcScoreType>();
        errorData = new List<Data>();
        lineRenderer = GetComponent<LineRenderer>();
        

        RouteInfoArray.Add(new RouteInfo("Route1", new CalcRouteType(GetLineCalc()), new DrawRouteType(draw_route1), new CalcMinPointType(getMinRouteFromLine)));
        RouteInfoArray.Add(new RouteInfo("Route2", new CalcRouteType(GetLineCalc()), new DrawRouteType(draw_route2), new CalcMinPointType(getMinRouteFromLine)));
        RouteInfoArray.Add(new RouteInfo("Route3", new CalcRouteType(GetLineCalc()), new DrawRouteType(draw_route3), new CalcMinPointType(getMinRouteFromLine)));
        RouteInfoArray.Add(new RouteInfo("Route4", new CalcRouteType(GetLineCalc()), new DrawRouteType(draw_route4), new CalcMinPointType(getMinRouteFromLine)));


        CalcScoreArray.Add(new CalcScoreType(calcScore1));
        CalcScoreArray.Add(new CalcScoreType(calcScore2));
        CalcScoreArray.Add(new CalcScoreType(calcScore3));
        CalcScoreArray.Add(new CalcScoreType(calcScore4));


        UI_dropdown.GetComponent<GetRouteDropdown>().StartRoutwDropDown();
        UI_dropdown.GetComponent<AlgorithmDropdown>().startAlgorithmDropdown();


        setStartPosition(0);
        drawNthStage(0);
        cur = lineRenderer.GetPosition(0);
        next = lineRenderer.GetPosition(1);

      
        errorText.text = "0";
        currentTime = 0;
        TimeText.text = "0";
        trial = 0;
        timerText.text = "3";
        waitTime = 2;
        isWait = false;
        maxTrial =0 ;

        routeNum = 0;
        calcNum = 0;
        frequencyIndex = 0;
        frequencyList = new int[3];
        frequencyList[0] = 250;
        frequencyList[1] = 100;
        frequencyList[2] = 50;

        tryNextIndex = new List<CalcIndex>();

        tryNextIndex.Add(new CalcIndex(tryNextIndexInPractice));
        tryNextIndex.Add(new CalcIndex(tryNextIndexInMain));

        GameObject subjectName = GameObject.Find("EmptyObject");
        testMode = -1;
        testMode = subjectName.GetComponent<intro>().getTestType();
        printv("testMode", testMode);


        fileName = "Assets/test1111";
        fileName = "Assets/" + subjectName.GetComponent<intro>().getName();
        if (testMode == 0) 
        {
            maxTrial = 12;
            fileName = fileName + "_No_vib";
        }
        else 
        {
            maxTrial = 4 * 3 * 4;
            fileName = fileName + "_vib";
        }
        printv("fileName", fileName);
        if (false == File.Exists(fileName))
        {
            sw = new StreamWriter(fileName + ".txt");
        }
        
        trialText.text =  "0 / "+maxTrial.ToString() ;



        
        currentError = 0.0f;


    }


    // Update is called once per frame
    void Update()
    {
        if (isWait == false)
        {
            
            time_increase();
            currentError = getErrorFromRoute(routeNum);
            currentError = CalcScoreArray[calcNum](currentError);
            if (testMode == 1)
                SineWave.GetComponent<sinewave>().setAmplitude(currentError);
            else
                SineWave.GetComponent<sinewave>().setAmplitude(0.0f);
            frameCount = frameCount + 1;
            calc_errorAvg(currentError);
            errorText.text = errorAvg.ToString();
            set_next_and_check();
        }
        else 
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                isWait = false;
                waitTime = 2;
                timerText.text = "START";
            }
            timerText.text = waitTime.ToString();
            errorText.text = "0";
            TimeText.text = "0";
            SineWave.GetComponent<sinewave>().setAmplitude(0);

        }

    }
    private void set_next_and_check() 
    {
        bool isRouteEnd2 = isEndPoint() && (routeNum == 3) && currentTime > 8.0f;

        if (isNextPoint() == true || isRouteEnd2)
        {
            Debug.Log("isNextPoint() == true");
            bool isRouteEnd = setNextPoint();
            

            if (isRouteEnd == true || isRouteEnd2 == true)
            {
                // TEST_i -> routeNum;
                bool isNext = tryNextIndex[testMode]();

                errorData.Add(new Data(errorAvg, currentTime));
                if (isNext == false) 
                {
                    
                    writeResult();
                    Destroy(GameObject.Find("EmptyObject"));
                    SceneManager.LoadScene("StartScene");
                }

                setStartPosition(routeNum);
                drawNthStage(routeNum);
                errorAvg = 0;
                frameCount = 0;
                time_init();
                isWait = true;

                updateTrial();

                if (routeNum == 0 && false) 
                {
                    writeResult();

                    Destroy(GameObject.Find("EmptyObject"));
                    SceneManager.LoadScene("StartScene"); 
                }
                

            }
            else
            {
                // NONE ACTIVATE
            }
            
        }
        else
        {
            printv("isNextPoint() == true", isNextPoint());
        }
    }
    private void updateTrial() 
    {
        trial = trial + 1;
        trialText.text = trial.ToString() + " / " + maxTrial.ToString();
    }

    private void writeResult() 
    {
        sw.WriteLine("Error  time");
        foreach (Data d in errorData) 
        {
            sw.WriteLine(d.errorAvg + "       " + d.time);        
        }

        sw.WriteLine("Error");
        foreach (Data d in errorData)
        {
            sw.WriteLine(d.errorAvg );
        }

        sw.Flush();
        sw.Close();

    }

    private bool tryNextIndexInMain() 
    {
        routeNum = (routeNum + 1) % 4;

        if (routeNum == 0) 
        {
            calcNum = (calcNum + 1) % 4;
            if (calcNum == 0) 
            {
                frequencyIndex = (frequencyIndex + 1) % 3;
                SineWave.GetComponent<sinewave>().SetFrequency(frequencyList[frequencyIndex]);
                if (frequencyIndex == 0) 
                {
                    return false;
                }
            }

        }
        return true;
    }

    private bool tryNextIndexInPractice() 
    {
        routeNum = (routeNum + 1) % 4;

        if (routeNum == 0)
        {
            calcNum = (calcNum + 1) % 3;
            if (calcNum == 0) 
            {
                return false;
            }
        }
        return true;
    }

    private void time_increase() 
    {
        currentTime += Time.deltaTime;
        TimeText.text = currentTime.ToString();
    }
    private void time_init() 
    {
        currentTime = 0.0f;
        TimeText.text = currentTime.ToString();
    }

    private float calc_errorAvg(float error) 
    {
        
        if (frameCount == 1)
        {
            errorAvg = error;
        }
        else 
        {
            float oldCount = (float)frameCount - 1.0f;
            float newCount = (float)frameCount ;
            errorAvg = errorAvg * (oldCount / newCount) + error / newCount;
        }
        return errorAvg;
    }
    /* Line */
    private void draw_route1() 
    {
        lineRenderer.SetColors(Color.red, Color.red);
        lineRenderer.SetWidth(0.1f, 0.1f);

        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(0, Player.GetComponent<Player>().initPlaceAtRound);
        lineRenderer.SetPosition(1, Goal.GetComponent<Goal>().initPlaceAtRound);

        passIndex = 0;
        isPassed = new bool[lineRenderer.positionCount];
    }
    /* Squar Route */
    private void draw_route2()
    {
        /* Draw real code */
        int n = 0;
        lineRenderer.SetColors(Color.red, Color.red);
        lineRenderer.SetWidth(0.1f, 0.1f);

        lineRenderer.positionCount = 5;

        lineRenderer.SetPosition(n++, Player.GetComponent<Player>().initPlaceAtRound);
        lineRenderer.SetPosition(n++, new Vector3(-10, -10, 0));
        lineRenderer.SetPosition(n++, new Vector3(10, -10, 0));
        lineRenderer.SetPosition(n++, new Vector3(10, 10, 0));
        lineRenderer.SetPosition(n, Goal.GetComponent<Goal>().initPlaceAtRound);

        passIndex = 0;
        isPassed = new bool[lineRenderer.positionCount];
    }

    /* Line base route */
    private void draw_route3() 
    {
        /* Draw real code */
        int n = 0;
        lineRenderer.SetColors(Color.red, Color.red);
        lineRenderer.SetWidth(0.1f, 0.1f);


        lineRenderer.positionCount = 8;

        lineRenderer.SetPosition(n++, Player.GetComponent<Player>().initPlaceAtRound);
        lineRenderer.SetPosition(n++, new Vector3(-10,0,0) );
        lineRenderer.SetPosition(n++, new Vector3(0, 0, 0));
        lineRenderer.SetPosition(n++, new Vector3(0, 10, 0));
        lineRenderer.SetPosition(n++, new Vector3(10, 10, 0));
        lineRenderer.SetPosition(n++, new Vector3(10, -3, 0));
        lineRenderer.SetPosition(n++, new Vector3(0, -3, 0));
        lineRenderer.SetPosition(n, Goal.GetComponent<Goal>().initPlaceAtRound);

        passIndex = 0;
        isPassed = new bool[lineRenderer.positionCount];

    }
    /* Line base route */
    private void draw_route4()
    {
        /* Draw real code */
        int n = 0;
        lineRenderer.SetColors(Color.red, Color.red);
        lineRenderer.SetWidth(0.1f, 0.1f);


        lineRenderer.positionCount = 0;
        int angle = 90;
        bool isStart = true;
        float r = 10.0f;
        for (angle = 90; (angle % 360) != 90 || isStart == true ; angle = angle + 1) 
        {
            double D2R = angle * Math.PI / 180.0f;
            isStart = false;
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(n++,new Vector3(r * Convert.ToSingle(Math.Cos(D2R)), r * Convert.ToSingle(Math.Sin(D2R)),0 ));
            
        }

        passIndex = 0;
        isPassed = new bool[lineRenderer.positionCount];
  
    }



    public bool setNextPoint() 
    {
        int posCount = lineRenderer.positionCount;
        Vector3 temp;
        isPassed[passIndex] = true;
        /* See next point */
        passIndex = passIndex + 1;
        /* if it is last, stop*/
        if (posCount == passIndex)
        {
            return true;
        }
        else 
        {

            /* */
            cur = next;
            next = lineRenderer.GetPosition(passIndex);
            return false;
        }
    }

    public bool isEndPoint() 
    {
        float dis = getDistanceToEnd();
        if (dis < 0.5f)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    public bool isNextPoint() 
    {
        float dis = getDistanceToNext();
        
        if (dis < 1.3f)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }


    public float getErrorFromRoute(int i)
    {
        float result;
        printv("i",i);
        if (i >= 0 && i <= 2) {
            result = getMinRouteFromAnyLine(cur, next);
            printv("cur", cur);
            printv("next", next);
            printv("result", result);
        }
        else 
        {
            result = getMinRouteFromCircle();
        }
        return result;
    }

    public float getDistanceToNext() 
    {
        return Vector3.Distance(Player.transform.position, next);
    }
    public float getDistanceToEnd()
    {
        int endPointIndex = lineRenderer.positionCount - 1;
        return Vector3.Distance(Player.transform.position, lineRenderer.GetPosition(endPointIndex));
    }
    public float getMinRouteFromLine()
    {
        Vector3 p = Player.transform.position;

        Vector3 pl = p1;
        Vector3 pr = p2;
        if (pl.y > pr.y) {
            Vector3 temp = pl;
            pl = pr;
            pr = temp;

        }
        else {
            ;
        }

        /* find the orthogonal 기울기. */
        float a = (p2.y - p1.y) / (p2.x - p1.x);
        a = -1 / a;

        float b1 = pl.y - a * pl.x;
        float b2 = pr.y - a * pr.x;

        float result = 0.0f;
        if (p.y < a * p.x + b1)
        {
            result = calcDistance(p, pl);
            Debug.Log("Distance from line  Low part: " + result);
        }
        else if (a * p.x + b2 < p.y)
        {
            result = calcDistance(p, pr);
            Debug.Log("Distance from line  High part: " + result);
        }
        else {

            Double degree = Convert.ToDouble(Vector3.Angle(pr - pl, p - pl));
            Double radian = degree * Math.PI / 180.0;

            //Debug.Log("Sin x : " + Convert.ToSingle(Math.Sin(radian)));
            result = Vector3.Distance(pl, p) * Convert.ToSingle(Math.Sin(radian));
            Debug.Log("Distance from line  Middle part: " + result);
        }

        return result;
    }

    public float getMinRouteFromCircle() 
    {
        // p1, p2
        Vector3 p = Player.transform.position;
        
        middle_point = new Vector3(0, 0, 0);
        float dis = Vector3.Distance(middle_point,Player.transform.position);
        float r = 10.0f;

        float result = 0.0f;

        result = Convert.ToSingle(Math.Abs(dis - r));

        printv("cir min distance",result);
        return result ;
        
    }



    public float getMinRouteFromAnyLine(Vector3 basePoint1, Vector3 basePoint2)
    {
        Vector3 p = Player.transform.position;

        Vector3 pl = basePoint1;
        Vector3 pr = basePoint2;
        if (pl.x > pr.x)
        {
            Vector3 temp = pl;
            pl = pr;
            pr = temp;

        }
        else
        {
            ;
        }

        /* find the orthogonal 기울기. */
        float a = (pl.y - pr.y) / (pl.x - pr.x);
        a = -1 / a;

        float b1 = pl.y - a * pl.x;
        float b2 = pr.y - a * pr.x;

        float result = 0.0f;
        if (pl.y < pr.y)
        {
            if (p.y < a * p.x + b1)
            {
                result = calcDistance(p, pl);
            }
            else if (a * p.x + b2 < p.y)
            {
                result = calcDistance(p, pr);
            }
            else
            {
                Double degree = Convert.ToDouble(Vector3.Angle(pr - pl, p - pl));
                Double radian = degree * Math.PI / 180.0;
                result = Vector3.Distance(pl, p) * Convert.ToSingle(Math.Sin(radian));

            }
        }
        else 
        {
            if (p.y > a * p.x + b1)
            {
                result = calcDistance(p, pl);
            }
            else if (a * p.x + b2 > p.y)
            {
                result = calcDistance(p, pr);
            }
            else
            {
                Double degree = Convert.ToDouble(Vector3.Angle(pr - pl, p - pl));
                Double radian = degree * Math.PI / 180.0;
                result = Vector3.Distance(pl, p) * Convert.ToSingle(Math.Sin(radian));

            }

        }
        return result;
    }

    public void setStartPosition(int n)
    {
        Vector3 start;
        Vector3 end;

        if (n == 0)  /* draw route 1 */
        {
            start = new Vector3(-10, -10, 0);
            end = new Vector3(15, 10, 0);
            Player.transform.position = start;
            Goal.transform.position = end;
            Player.GetComponent<Player>().initPlaceAtRound = start;
            Goal.GetComponent<Goal>().initPlaceAtRound = end;
        }
        else if (n == 1) /* draw route 2 squar */
        {
            start = new Vector3(-10, 10, 0);
            end = new Vector3(-10, 10, 0);
            Player.transform.position = start;
            Goal.transform.position = end;
            Player.GetComponent<Player>().initPlaceAtRound = start;
            Goal.GetComponent<Goal>().initPlaceAtRound = end;
        }
        else if (n ==  2) /* draw route 3, Maze */
        {
            start = new Vector3(-10, 10, 0);
            end = new Vector3(0, -10, 0);
            Player.transform.position = start;
            Goal.transform.position = end;
            Player.GetComponent<Player>().initPlaceAtRound = start;
            Goal.GetComponent<Goal>().initPlaceAtRound = end;

        }
        else if (n == 3) /* draw route 4, Circle */
        {
            start = new Vector3(0, 10, 0);
            end = new Vector3(0, 10, 0);
            Player.transform.position = start;
            Goal.transform.position = end;
            Player.GetComponent<Player>().initPlaceAtRound = start;
            Goal.GetComponent<Goal>().initPlaceAtRound = end;
        }
        else
        {
            Debug.Log("Can't reach here");
            /*Can not reach here*/
        }
    }



    public float calcDistance(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2);
    }

    public float calcScore1(float x) 
    {
        /* y = a x , if x = 0 , value is 0*/

        float a = 1.0f / 2.0f;
        
        
        float result = a * x;

        if (result > 1)
        {
            result = 1;
        }
        else if (result < 0)
        {
            result = 0;
        }
        else {
            ;
        }

        //Debug.Log("Print calcScore1 result score : " + result);
        return result;

    }

    public float calcScore2(float x) 
    {
        float a = 1.0f;
        
        /* x of distance */
        // float x = RouteInfoArray[n].calcMinDis();
        float result = 0.0f;
        if (x >= 0.5f)
        {
            result = 1.0f;
        }
        else 
        {
            ;
        }

        Debug.Log("calcScore2  :  " +result);

        return result;
    }

    public float calcScore3(float x) {

        float a = 1 / 10.0f;
        
        /* x of distance */
        //float x = RouteInfoArray[n].calcMinDis();
        float result = a * Convert.ToSingle(Math.Pow(2,x)) ;
        printv("result",result);
        if (result < 0)
        {
            result = 0.0f;
        }
        else if (result > 1.0f) {
            result = 1.0f;
        }
        else 
        {
            ; 
        }

        Debug.Log("calcScore3 : " + result);

        return result;

    }

    public float calcScore4(float x) 
    {
        float a = 1 / 4.0f;
        
        /* x of distance */
        //float x = RouteInfoArray[n].calcMinDis();
        float result = Convert.ToSingle(Math.Log( x + 1 ));

        if (result > 1.0f)
            result = 1;
        else if (result < 0.0f)
            result = 0;
        else {
            ;
        }

        return result;
    }


    public void drawCurrentStage()
    {
        int n = UI_dropdown.GetComponent<GetRouteDropdown>().GetDropDownValue();
        Debug.Log("drawCurrentStage : " + n);
        RouteInfoArray[n].draw();
        cur = lineRenderer.GetPosition(0);
        next = lineRenderer.GetPosition(1);
    }
    public void drawNthStage(int n)
    {
        Debug.Log("drawCurrentStage : " + n);
        RouteInfoArray[n].draw();
        cur = lineRenderer.GetPosition(0);
        next = lineRenderer.GetPosition(1);
    }

  
    private CalcRouteType GetLineCalc()
    {
        Vector3 p1, p2;

        p1 = Player.GetComponent<Player>().initPlaceAtRound;
        p2 = Goal.GetComponent<Goal>().initPlaceAtRound;

        /*  y = a x   +   b */

        CalcRouteType temp = delegate (Vector3 p1, Vector3 p2, float f) {
            float a = (p1.y - p2.y) / (p1.x - p2.x);
            float b = p1.y - a * p1.x;
            Debug.Log("A : " + a);

            Debug.Log("B : " + b);
            return a * f + b;
        };

        return temp;

    }



    private void printv(string name, Vector3 v) 
    {
        Debug.Log(name + " : " + v); 
    }
    private void printv(string name, float f) 
    {
        Debug.Log(name + " : " + f);
    }
    private void printv(string name, int i)
    {
        Debug.Log(name + " : " + i);
    }
    private void printv(string name, string s)
    {
        Debug.Log(name + " : " + s);
    }
    private void printv(string name, bool b)
    {
        Debug.Log(name + " : " + b);
    }

}
