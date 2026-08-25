using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum ApplicationType
{
    Messages,
    Contacts,
    Calendar,
    Notes,
    Clock,
    Settings,
    Photos,
    Map,
    Telephone,
    Camera,
    Internet,
    Hack
}
abstract public class BaseApplication : MonoBehaviour
{
    protected Canvas _canvas;
    Canvas _phoneCanvas;
    public ApplicationType _appType;
    private Header _header;
    private RectTransform _rectTransform;
    [SerializeField] private Sprite _logo;
    [SerializeField, Range(1f, 1.5f)] private float _closingTime;
    [SerializeField] bool _isUnlocked=true;

    public Sprite Logo { get => _logo;}
    public bool IsUnlocked { get => _isUnlocked; }

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        _phoneCanvas = PhoneManager.Instance.gameObject.GetComponent<Canvas>();
        _header = FindFirstObjectByType<Header>();

    }
    abstract public void SetUp(StoryAppSetup setup);
    public virtual void PostSetUp()
    {

    }
    abstract public void CloseCurrent();
    //indentedfield = serialzlizedIs=true:


    public void CloseApp()
    {
        if (_canvas.isActiveAndEnabled)
        {
            _header.AppChangedUpdate(true,false);
            _canvas.enabled = false;
            //_phoneCanvas.enabled = true;
            PhoneManager.Instance.ChangeDepth(PhoneManager.AppDepth.phone);
        }
    }
    /// <summary>
    /// ne marche pas à cuase du mode du canvas donc pas moyyen de faire l'effet correctement.
    /// </summary>
    /// <returns></returns>
     IEnumerator CloseAppEffect()
    {
        float timer = 0;
        while (timer < _closingTime)
        {
            _rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timer / _closingTime);
            print(_rectTransform.localScale);
            timer += Time.deltaTime;
            yield return null;
        }
        _rectTransform.localScale = Vector3.zero;
        yield return new WaitForEndOfFrame();
        _rectTransform.localScale = Vector3.one;
        yield return null;
    }
}
