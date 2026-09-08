using System;
using Unity.Services.LevelPlay;
using UnityEngine;

public class LevelPlayAdsManager : MonoBehaviour
{
    [Header("App Key")]
    [SerializeField] private string _androidAppKey;
    [SerializeField] private string _iosAppKey;

    [Header("Banner Ad Unit ID")]
    [SerializeField] private string _androidBannerAdUnitID;
    [SerializeField] private string _iosBannerAdUnitID;

    [Header("Interstitial Ad Unit ID")]
    [SerializeField] private string _androidInterstitialAdUnitID;
    [SerializeField] private string _iosInterstitialAdUnitID;

    [Header("Rewarded Ad Unit ID")]
    [SerializeField] private string _androidRewardedAdUnitID;
    [SerializeField] private string _iosRewardedAdUnitID;

    private LevelPlayBannerAd _bannerAd;
    private LevelPlayInterstitialAd _interstitialAd;
    private LevelPlayRewardedAd _rewardedAd;

    private string _appKey
    {
        get
        {
#if UNITY_ANDROID
            return _androidAppKey;
#elif UNITY_IOS
            return iosAppKey;
#else
            return string.Empty;
#endif
        }
    }
    private string _bannerAdUnitID
    {
        get
        {
#if UNITY_ANDROID
            return _androidBannerAdUnitID;
#elif UNITY_IOS
            return iosBannerAdUnitID;
#else
            return string.Empty;
#endif
        }
    }
    private string _interstitialAdUnitID
    {
        get
        {
#if UNITY_ANDROID
            return _androidInterstitialAdUnitID;
#elif UNITY_IOS
            return iosInterstitialAdUnitID;
#else
            return string.Empty;
#endif
        }
    }
    private string _rewardedAdUnitID
    {
        get
        {
#if UNITY_ANDROID
            return _androidRewardedAdUnitID;
#elif UNITY_IOS
            return iosRewardedAdUnitID;
#else
            return string.Empty;
#endif
        }
    }

    public static LevelPlayAdsManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        LevelPlay.ValidateIntegration();

        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        LevelPlay.Init(_appKey);
    }

    
    private void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
    {
        CreateBannerAd();
        CreateInterstitialAd();
        CreateRewardedAd();
        Debug.Log("LevelPlay Init Success");
        
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError error)
    {
        Debug.Log("LevelPlay Init Failed: " + error);
    }

#region AdCreation
    #region BannerAd

    void CreateBannerAd()
    {
        var config = new LevelPlayBannerAd.Config.Builder().SetPosition(LevelPlayBannerPosition.BottomCenter).Build();

        _bannerAd = new LevelPlayBannerAd(_bannerAdUnitID, config);

        _bannerAd.OnAdLoaded += BannerOnAdLoadedEvent;
        _bannerAd.OnAdLoadFailed += BannerOnAdLoadFailedEvent;
        _bannerAd.OnAdDisplayed += BannerOnAdDisplayedEvent;
        _bannerAd.OnAdDisplayFailed += BannerOnAdDisplayFailedEvent;
        _bannerAd.OnAdClicked += BannerOnAdClickedEvent;
        _bannerAd.OnAdCollapsed += BannerOnAdCollapsedEvent;
        _bannerAd.OnAdLeftApplication += BannerOnAdLeftApplicationEvent;
        _bannerAd.OnAdExpanded += BannerOnAdExpandedEvent;
    }
    public void ShowBanner()
    {
        _bannerAd.LoadAd();
    }

    public void DestroyBanner()
    {
        _bannerAd.DestroyAd();
    }

    void BannerOnAdLoadedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdLoadFailedEvent(LevelPlayAdError ironSourceError) { }
    void BannerOnAdClickedEvent(LevelPlayAdInfo adInfo)
    {
        Debug.Log("Click banner ad");
    }
    void BannerOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error) { }
    void BannerOnAdCollapsedEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdLeftApplicationEvent(LevelPlayAdInfo adInfo) { }
    void BannerOnAdExpandedEvent(LevelPlayAdInfo adInfo) { }

    #endregion

    #region InterstitialAd

    void CreateInterstitialAd()
    {
        _interstitialAd = new LevelPlayInterstitialAd(_interstitialAdUnitID);

        _interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        _interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        _interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        _interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        _interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        _interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        _interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;

        //LoadInterstititalAd();
    }

    public void LoadInterstititalAd()
    {
        _interstitialAd.LoadAd();
        Debug.Log("Interstitial Ad Loaded");
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd.IsAdReady())
        {
            _interstitialAd.ShowAd();
            Debug.Log("Interstitial Ad Shown");
        }
    }

    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        //LoadInterstititalAd();
    }
    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error) { }
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        //LoadInterstititalAd();
    }
    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo) { }
    #endregion

    #region RewardedAd

    void CreateRewardedAd()
    {
        _rewardedAd = new LevelPlayRewardedAd(_rewardedAdUnitID);

        _rewardedAd.OnAdLoaded += RewardedOnAdLoadedEvent;
        _rewardedAd.OnAdLoadFailed += RewardedOnAdLoadFailedEvent;
        _rewardedAd.OnAdDisplayed += RewardedOnAdDisplayedEvent;
        _rewardedAd.OnAdDisplayFailed += RewardedOnAdDisplayFailedEvent;
        _rewardedAd.OnAdRewarded += RewardedOnAdRewardedEvent;
        _rewardedAd.OnAdClosed += RewardedOnAdClosedEvent;
        // Optional
        _rewardedAd.OnAdClicked += RewardedOnAdClickedEvent;
        _rewardedAd.OnAdInfoChanged += RewardedOnAdInfoChangedEvent;

        //LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        _rewardedAd.LoadAd();
        Debug.Log("Rewarded Ad Loader");
    }

    public void ShowRewardedAd()
    {
        if (_rewardedAd.IsAdReady())
        {
            _rewardedAd.ShowAd();
            Debug.Log("Rewarded Ad Shown");
        }
    }

    void RewardedOnAdLoadedEvent(LevelPlayAdInfo adInfo) { }
    void RewardedOnAdLoadFailedEvent(LevelPlayAdError error)
    {
        //LoadRewardedAd();
    }
    void RewardedOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { }
    void RewardedOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error) { }
    void RewardedOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward adReward)
    {
        Debug.Log($"Reward earned: {adReward.Amount} {adReward.Name}");
        SaveManager.Instance.AddEnergy(adReward.Amount);
    }
    void RewardedOnAdClosedEvent(LevelPlayAdInfo adInfo)
    {
        //LoadRewardedAd();
    }
    void RewardedOnAdClickedEvent(LevelPlayAdInfo adInfo) { }
    void RewardedOnAdInfoChangedEvent(LevelPlayAdInfo adInfo) { }
    #endregion
    #endregion
}
