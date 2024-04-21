using AnyCAD.Foundation;

namespace AnyCAD.Rapid.Dynamo.Sandbox
{
    /// <summary>
    /// 自定义的文档模板
    /// </summary>
    public class RapidDynamoDocTemplate : DocumentTemplate
    {
        public static readonly string DocType = "RapidDynamoDocTemplate";
        public RapidDynamoDocTemplate() 
            : base(DocType)
        {

        }

        /// <summary>
        /// 定义文档创建后的模板行为
        /// </summary>
        /// <param name="document"></param>
        /// <param name="viewName"></param>
        public override void Initialize(Document document, string viewName)
        {
            base.Initialize(document, viewName);

            // 创建默认材质对象
            var material = MaterialElement.Create(document);
            material.SetName("Default");
            material.SetColor(new Vector3(0.3f, 0.3f, 0.3f));
            material.SetOpacity(0.1f);
        }
    }
}
