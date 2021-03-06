﻿//MIT, 2015-present, WinterDev   
using LayoutFarm.Scripting;

namespace LayoutFarm.WebDom
{
    public interface INode
    {
    }

    [JsType]
    public interface IHtmlDocument
    {
        //TODO: 
        //current version we not support overload
        //resolve this
        IHtmlElement rootNode { get; }
        IHtmlElement getElementById(string id);
        ITextNode createTextNode(string nodeContent);
        IHtmlElement createElement(string nodeName);
        IHtmlDocument createDocumentFragment();
        IHtmlElement createShadowRootElement();

    }
    [JsType]
    public interface IHtmlElement : INode
    {
        string getAttribute(string attrName);
        void setAttribute(string attrName, string value);
        void appendChild(INode childNode);
        void attachEventListener(string eventName, HtmlEventHandler handler);
        void detachEventListener(string eventName, HtmlEventHandler handler);
        string innerHTML { get; set; }
        string id { get; }
        void clear();
        //----------------------
        void removeChild(DomNode childNode);
        void getGlobalLocation(out int x, out int y);
        //----------------------
        void setData(string key, string value);
        string getData(string key);
        //----------------------
    }

    [JsType]
    public interface IHasValue
    {
        string Value { get; set; }
    }
    [JsType]
    public interface IHtmlOptionElement : IHtmlElement, IHasValue
    {
    }




    [JsType]
    public interface IHtmlInputElement : IHtmlElement
    {
        [JsProperty("name")]
        string InputName { get; set; }
        [JsProperty("value")]
        string Value { get; set; }
        string inputType { get; }
        void Focus();
    }
    [JsType]
    public interface IHtmlTextArea : IHtmlElement
    {
        [JsProperty("value")]
        string Value { get; set; }
        void Focus();
    }


    [JsType]
    public interface ITextNode : INode
    {
    }

    [JsType]
    public interface INodeList
    {

    }
}