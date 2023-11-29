using System;

namespace Laya.mtc_mvc_core.Core.Services.PdfConverter.Service;

public interface IPdfConverterService
{
    byte[] ConvertToPdf(Uri uriSource);
    //void ConvertToPdf(Uri uriSource, Stream streamTarget);
    //void ConvertToPdf(Uri uriSource, string fileNameTarget);

}