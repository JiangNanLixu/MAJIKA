﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine.EventSystems;
using MAJIKA.TextManager;
using System.Linq;
using TMPro;

public class ConversationUI : Singleton<ConversationUI>, IPointerClickHandler
{
    public float WPS = 10;
    public GameObject Wrapper;
    public TextMeshProUGUI TextRenderer;
    public Image ImageRenderer;
    public Graphic ContinueHint;
    public Sprite Transperant;
    public Talker[] Talkers;
    public IConversation Conversation;
    public bool Talking = false;

    DoubleBuffer<bool> touchScreenSkip = new DoubleBuffer<bool>();
    ConversationTextDefinition textDefinition = new ConversationTextDefinition();

    void Update()
    {
        touchScreenSkip.Update();
        touchScreenSkip.Value = false;
    }

    public void TouchScreenSkip()
    {
        touchScreenSkip.Value = true;
    }

    public IEnumerator StartConversationAsync(IConversation conversation, Talker[] talkers, bool lockPlayer = false, bool autoHide = true)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
        yield return StartCoroutine(StartConversationInternal(conversation, talkers, lockPlayer, autoHide));
    }

    IEnumerator StartConversationInternal(IConversation conversation, Talker[] talkers, bool lockPlayer = false, bool autoHide = true)
    {
        TextRenderer.text = "";
        Guid lockID;
        if (lockPlayer)
            lockID = GameSystem.Instance.PlayerInControl.GetComponent<EntityController>().Lock();
        MAJIKA.GUI.CoveredUI.Find("GameUI")?.HideAsync(.2f);

        textDefinition.Talkers = Talkers = talkers;
        Conversation = conversation;
        Talking = true;
        Wrapper.SetActive(true);
        foreach (var text in conversation)
        {
            yield return ShowSentence(text);
        }
        if (autoHide)
        {
            Talkers = null;
            Conversation = null;
            Wrapper.SetActive(false);
            yield return null;
            GameSystem.Instance.PlayerInControl.GetComponent<EntityController>().UnLock(lockID);
            MAJIKA.GUI.CoveredUI.Find("GameUI")?.ShowAsync(.2f);
        }
        // InputManager.Instance.Jumped = false;
        // InputManager.Instance.Jumped = false;
    }

    public void HideConversation()
    {
        Talkers = null;
        Conversation = null;
        Wrapper.SetActive(false);
        MAJIKA.GUI.CoveredUI.Find("GameUI")?.ShowAsync(.2f);
    }

    public void EndConversation()
    {
        Conversation = null;
    }

    public IEnumerator ShowSentence(string text)
    {
        var richTextElementRegEx = new Regex(@"(<\S+.*>.*</\S+>|<\S+.*>|.)");
        yield return null;
        var (talker, renderedText) = RenderSentence(text);
        if (talker)
            ImageRenderer.sprite = talker.Image;
        else
            ImageRenderer.sprite = Transperant;
        var secondsPerWord = 1 / WPS;
        var matches = richTextElementRegEx.Matches(renderedText);
        var displayText = "";
        for(var i=0;i<matches.Count;i++)
        {
            var match = matches[i];
            displayText += match.Value;
            TextRenderer.text = displayText;
            foreach(var t in Utility.Timer(secondsPerWord))
            {
                if (InputManager.Instance.Controller.Actions.Accept.WasPressedThisFrame() || touchScreenSkip.Value)
                    goto SkipAnim;
                yield return null;
            }
        }
      SkipAnim:
        TextRenderer.text = renderedText;

        yield return null;
        yield return Utility.ShowUI(ContinueHint, 0.2f);
        while (!InputManager.Instance.Actions.Accept.WasPressedThisFrame() && !touchScreenSkip.Value)
            yield return null;
        // yield return InputManager.Instance.Controller.Actions.Accept.WaitPerform();
        yield return Utility.HideUI(ContinueHint, 0);
    }
    (Talker Talker, string Text) RenderSentence(string textTemplate)
    {
        var headerReg = new Regex(@"\$\{(.*?)\}:");
        var reg = new Regex(@"\$\{(.*?)\}");
        var talkerTemplate = headerReg.Match(textTemplate)?.Groups[1].Value;
        Talker talker = null;
        switch(talkerTemplate)
        {
            case "player":
                talker = FindObjectOfType<Player>().GetComponent<Talkable>().Talker;
                break;
            default:
                try
                {
                    talker = Talkers[Convert.ToInt32(talkerTemplate)];
                }
                catch
                {
                    talker = null;
                }
                break;
        }
        var template = new Dictionary<string, string>();

        var text = TextManager.RenderText(textTemplate, textDefinition);
        return (talker, text);
    }

    string TemplateReplacer(string template)
    {
        var replacer = new Dictionary<string, Func<string>>();
        if (template == "player")
            return FindObjectOfType<Player>().GetComponent<Talkable>().Talker.Name;
        try
        {
            return Talkers[Convert.ToInt32(template)].Name;
        }
        catch
        {
            return $"{{{template}}}";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TouchScreenSkip();
    }

    class ConversationTextDefinition : ITextDefinition
    {
        public Talker[] Talkers;
        public string this[string id]
        {
            get
            {
                if (id == "player")
                    return GameSystem.Instance.PlayerInControl?.GetComponent<Talkable>()?.Talker.Name;

                int idx;
                if (!int.TryParse(id, out idx))
                    idx = -1;
                return Talkers.TryGet(Convert.ToInt32(idx))?.Name;
            }
        }


        public bool Has(string id)
        {
            if (id == "player")
                return GameSystem.Instance.PlayerInControl;
            int idx = -1;
            if (!int.TryParse(id, out idx))
                return false;
            return idx >= 0 && idx < Talkers.Length;
        }
    }

}

class Sentence
{
    public Talker Talker = null;
    public string Text = null;
}
