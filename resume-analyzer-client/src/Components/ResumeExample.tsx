import React from 'react';

const ResumeExample = ({googleDocsUrl, pdfFileUrl}:{googleDocsUrl?: string, pdfFileUrl:string}) => {
    return (
        <div className={"rounded-3xl w-full p-4 sm:p-8 flex gap-6 items-center justify-center flex-col"}>
            <div className={"flex flex-wrap items-center justify-center gap-4 sm:gap-6"}>
                {googleDocsUrl ? <a className={"btn-2"} target={"_blank"} href={googleDocsUrl}>Google docs</a> : null}
                <a target="_blank" href={`${pdfFileUrl}`} className={"btn-2"}>Open pdf</a>
                <a href={`${pdfFileUrl}`} className={"btn-2"} download>Download pdf</a>
            </div>
            <embed
                type={"application/pdf"}
                src={`${pdfFileUrl}#toolbar=0&view=Fit`}
                className={"rounded-2xl w-full h-[380px] md:w-[400px] md:h-[500px] lg:w-[43vw] lg:h-[700px] 2xl:w-[30vw]"}
                style={{ border: 'none' }}
            />
        </div>
    );
};

export default ResumeExample;