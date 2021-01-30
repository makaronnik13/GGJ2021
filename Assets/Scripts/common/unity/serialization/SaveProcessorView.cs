using System;
using System.IO;
using com.armatur.common.save;
using com.armatur.common.serialization;
using UnityEngine;
using Resources = UnityEngine.Resources;

namespace com.armatur.common.unity.serialization
{
    public class SaveProcessorView : MonoBehaviour
    {
        private readonly SaveProcessorUnity _processor = new SaveProcessorUnity();


        [SerializeField] public BasicMemberUnityView BasicMemberUnityView;
        [SerializeField] public EnumMemberUnityView EnumMemberUnityView;
        [SerializeField] public ComplexMemberUnityView ComplexMemberUnityView;
 
        private void Awake()
        {
            var textAsset = Resources.Load("Configuration/game") as TextAsset;
            if (textAsset != null)
            {
                var game = new SaveProcessorXml(textAsset.text).LoadObject<Game>();

//                var obj = CommandCost.CreateSimple(ResourceSource.All, new Cost(ResourceId.Life, 10));
                var obj = game; //.EventLibrary;
  
                SetType(obj.GetType());
                _processor.SaveObject(obj);
                var loadedGame = _processor.LoadObject<Game>();
                var saveProcessorXml = new SaveProcessorXml();
                saveProcessorXml.SaveObject(loadedGame);
                File.WriteAllText(Application.dataPath + "game_save.xml", saveProcessorXml.XDoc.ToString());
            }


//            var obj = new CommandList();
//            obj.AddCommand(CommandCost.CreateSimple(ResourceSource.All, new Cost(Resource.Life, 20)));
//            SetType(obj.GetType());
//            _processor.SaveObject(obj);
        }

        public void SetType(Type initialType)
        {
            var typeWrapper = TypeWrapperPool.Instance.GetWrapper(initialType);
            var memberView = CreateView(typeWrapper.Name(), typeWrapper, false);
            ((MonoBehaviour) memberView).gameObject.transform.SetParent(transform);
            _processor.SetRootView(memberView);
        }

        public IMemberView CreateView(string viewName, TypeWrapper typeWrapper, bool allowInheritance)
        {
            IMemberView memberView = null;
            if (typeWrapper is BasicTypeWrapper)
            {
                memberView = CreateBasicView(viewName, typeWrapper);
            }
            else if (typeWrapper is ComplexTypeWrapper)
            {
                var complexMemberUnityView = Instantiate(ComplexMemberUnityView);
                complexMemberUnityView.Init(viewName, typeWrapper, this, allowInheritance);
                memberView = complexMemberUnityView;
            }

            return memberView;
        }

        public IMemberView CreateView(MemberWrapper memberWrapper)
        {
            if (memberWrapper.IsOneValueView() || memberWrapper.TypeWrapper is BasicTypeWrapper)
            {
                return CreateBasicView(memberWrapper.Name, memberWrapper.TypeWrapper);
            }

            IMemberView res = null;
            if (memberWrapper.TypeWrapper is ComplexTypeWrapper)
            {
                var complexMemberUnityView = Instantiate(ComplexMemberUnityView);
                complexMemberUnityView.Init(memberWrapper, this);
                res = complexMemberUnityView;
            }

            return res;
        }

        private IMemberView CreateBasicView(string viewName, TypeWrapper typeWrapper)
        {
            IMemberView res;
            if (typeWrapper.Type.IsEnum)
            {
                var enumMemberUnityView = Instantiate(EnumMemberUnityView);
                enumMemberUnityView.Init(viewName, typeWrapper.Type);
                res = enumMemberUnityView;
            }
            else
            {
                var basicMemberUnityView = Instantiate(BasicMemberUnityView);
                basicMemberUnityView.Init(viewName);
                res = basicMemberUnityView;
            }

            return res;
        }
    }
}