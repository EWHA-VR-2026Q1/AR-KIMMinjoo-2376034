using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.Networking;

using UnityEngine.UI;



public class EX_GoogleMap : MonoBehaviour

{

    public RawImage MapImage;



    string BaseURL = "https://maps.googleapis.com/maps/api/staticmap?";

    string URL = "";



    public int zoom = 14;

    public int mapWidth = 400;

    public int mapHeight = 400;

    string APIKey = "AIzaSyBMYZAkXzsNcUnp - fJPi2mrHqPJ47B97YA";



    // Seoul City Hall: 37.566827, 126.978113 

    double latitude = 37.566827; // 위도(남북) 

    double longitude = 126.978113; // 경도 (동서) 



    private void Start()

    {

        LoadMap();

    }



    public void LoadMap()

    {

        StartCoroutine(ILoadMap());

    }



    IEnumerator ILoadMap()

    {

        // URL form: "https://maps.googleapis.com/maps/api/staticmap?center=Z%C3%BCrich&zoom=12&size=400x400&key=YOUR_API_KEY" 

        URL = BaseURL + "center=" + latitude + "," + longitude +

            "&zoom=" + zoom.ToString() +

            "&size=" + mapWidth.ToString() + "x" + mapHeight.ToString() +

            "&key=" + APIKey +

            "&maptype=terrain" + //terrain. hybrid, satellite, roadmap 

            // "&markers=size:mid%7Ccolor:red%7Clabel:H%7C" + latitude + "," + longitude; 

            "&markers=size:mid|color:red|label:H|" + latitude + "," + longitude;



        print("URL : " + URL);



        URL = UnityWebRequest.UnEscapeURL(URL);

        print($"UnEscapeURL: {URL}");



        UnityWebRequest www = UnityWebRequestTexture.GetTexture(URL);

        yield return www.SendWebRequest();



        if (www.error == null)

        {

            MapImage.texture = DownloadHandlerTexture.GetContent(www);

        }

        else

        {

            print("Failed");

            print(www.error);

        }

    }

}