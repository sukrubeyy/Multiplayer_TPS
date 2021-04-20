using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class VideoPlayerScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Canvas videoCanvas;
    public Canvas UICanvas;
    void Start()
    {
        videoPlayer.loopPointReached += VideoOver;
    }

  void VideoOver(UnityEngine.Video.VideoPlayer vp)
    {
        videoCanvas.gameObject.SetActive(false);
        UICanvas.gameObject.SetActive(true);
    }
}
