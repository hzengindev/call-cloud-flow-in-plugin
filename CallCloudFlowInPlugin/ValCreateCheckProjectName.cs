using Microsoft.Xrm.Sdk;
using System;

namespace CallCloudFlowInPlugin
{
    public class ValCreateCheckProjectName : IPlugin
    {
        IOrganizationService service;
        IPluginExecutionContext context;
        ITracingService trace;

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                Initial(serviceProvider);

                var entity = (Entity)context.InputParameters["Target"];
                new ValCreateCheckProjectNameHandler(service).Handle(entity);
            }
            catch (Exception ex)
            {
                trace.Trace(ex.Message);
                throw new InvalidPluginExecutionException($"Execuion of {typeof(ValCreateCheckProjectName).Name} | Message: {ex.Message}", ex);
            }
        }

        private void Initial(IServiceProvider serviceProvider)
        {
            try
            {
                context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                service = ((IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory))).CreateOrganizationService(context.UserId);
                trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("GetOrganizationService Error", ex);
            }
        }
    }
}
