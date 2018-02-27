namespace KsWare.AppVeyorClient.Api.Contracts {

	public class SecurityDescriptorData {

		public AccessRightDefinitionData[] AccessRightDefinitions { get; set; }

		public RoleAceData[] RoleAces { get; set; }

	}
}
