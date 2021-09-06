using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uco.Infrastructure.Repositories;
using Uco.Models;
using System.Reflection;
using Uco.Infrastructure.Livecycle;

namespace Uco.Infrastructure.Providers
{
    public class UcoLocalizationProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(
                             IEnumerable<Attribute> Attributes,
                             Type ProertyContainerType,
                             Func<object> ModelAccessor,
                             Type ModelType,
                             string PropertyName)
        {
            ModelMetadata PropertyMetadata = null;
            try
            {
                PropertyMetadata = base.CreateMetadata(Attributes, ProertyContainerType, ModelAccessor, ModelType, PropertyName);
            }
            catch
            {
                PropertyMetadata = base.CreateMetadata(new List<Attribute>(), ProertyContainerType, ModelAccessor, ModelType, PropertyName);
            }

            if (ProertyContainerType == null) return PropertyMetadata;

            string ProertyName = string.Empty;
            string ProoertyLocalizedText = string.Empty;
            string ProoertyKeyType = string.Empty;

            ProoertyKeyType = ProertyContainerType.Name;
            if (ProertyContainerType.BaseType != null && ProertyContainerType.BaseType == typeof(AbstractPage))
            {
                ProoertyKeyType = ProertyContainerType.BaseType.Name;
            }

            foreach (var Attr in Attributes)
            {
                if (Attr != null)
                {
                    //translate Prompt Attribute 
                    if (Attr is DisplayAttribute)
                    {
                        string PropertyPromt = ((DisplayAttribute)Attr).Prompt;
                        if (!string.IsNullOrEmpty(PropertyPromt))
                        {
                            var SystemName = string.Format("Models.{0}.{1}", "Prompt", PropertyPromt);
                            PropertyPromt = RP.T(SystemName, PropertyPromt);
                            PropertyMetadata.Watermark = PropertyPromt;
                        }
                    }
                    //translate ValidationAttribute 
                    else if (Attr is ValidationAttribute)
                    {
                        string ProertyErrorMessage = ((ValidationAttribute)Attr).ErrorMessage;
                        if (!string.IsNullOrEmpty(ProertyErrorMessage))
                        {
                            string AttrTypeName = Attr.GetType().Name;
                            
                            var SystemName = string.Format("Models.{0}.{1}.{2}", ProoertyKeyType, PropertyName, AttrTypeName);
                            ProertyErrorMessage = RP.T(SystemName, ProertyErrorMessage);
                            ((ValidationAttribute)Attr).ErrorMessage = ProertyErrorMessage;
                            
                        }
                    }
                }
            }

            string ProoertySystemName = string.Format("Models.{0}.{1}", ProoertyKeyType, PropertyName);
            PropertyMetadata.DisplayName = RP.T(ProoertySystemName, PropertyName);
            return PropertyMetadata;
        }
    }
}