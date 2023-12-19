using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public MenuScreen[] menus;
    public InputActionReference EscapeAction;

    private Stack<Button> _buttonsQueue = new();
    private Stack<MenuScreen> _menusQueue = new();

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EscapeAction.action.performed += DeselectMenu;
        EscapeAction.action.Enable();
    }

    private void OnDisable()
    {
        EscapeAction.action.performed -= DeselectMenu;
        EscapeAction.action.Disable();
    }

    private void Update()
    {
        //string s = string.Empty;
        //for (int i = 0; i < _buttonsQueue.Count; ++i)
        //{
        //    s += _buttonsQueue.ToArray()[i].name + ", ";
        //}
        //Debug.Log("QUEUE: " + s);           

    }

    #region OLD_CODE
    public void CloseAll()
    {
        for (int i = 0; i < menus.Length; ++i)
        {
            menus[i].gameObject.SetActive(false);
        }
    }

    public void CloseAllAndOpen(MenuScreen menu)
    {
        CloseAll();
        OpenMenu(menu);
    }
 
    public void OpenMenu(MenuScreen menu)
    {        
        menu.gameObject.SetActive(true);
    }
    #endregion

    #region BUTTON_MANAGEMENT
    public void SelectButton(Button button)
    {
        Button selectedButton;
        if (null != EventSystem.current.currentSelectedGameObject && 
            EventSystem.current.currentSelectedGameObject.TryGetComponent(out selectedButton))
        {
            if (_buttonsQueue.Count > 0 && _buttonsQueue.Peek().gameObject.transform.parent == selectedButton.gameObject.transform.parent)
            {
                _buttonsQueue.Pop();
            }
            _buttonsQueue.Push(selectedButton);
        }       

        _buttonsQueue.Push(button);
        RefreshButtonSelection();
    }

    public void DeselectButton(Button button)
    {
        if (_buttonsQueue.Count > 0 && button == _buttonsQueue.Peek())
        {
            _buttonsQueue.Pop();  
        }
        RefreshButtonSelection();
    }

    public void RefreshButtonSelection()
    {
        if (_buttonsQueue.Count > 0)
        {
            _buttonsQueue.Peek().Select();
        }
    }
    #endregion

    #region MENU_MANAGEMENT
    public void SelectMenu(MenuScreen menu)
    {
        _menusQueue.Push(menu);
        RefreshButtonSelection();
    }

    // Deselecting because the menu is hidden, or was closed
    public void DeselectMenu(MenuScreen menu)
    {
        if (_menusQueue.Count > 0 && menu == _menusQueue.Peek())
        {
            _menusQueue.Pop();
        }
    }

    // Deselecting via the ESC button 
    public void DeselectMenu(InputAction.CallbackContext obj)
    {
        if (_menusQueue.Count > 0)
        {
            var menu = _menusQueue.Peek();
            _menusQueue.Pop();
            menu.SelectDefaultButton();
        }
    }
    #endregion

}
