namespace KsWare.AppVeyor.Api.Contracts {

	public class RoleAceData {
		public int RoleId { get; set; }
		public string Name { get; set; }
		public bool IsAdmin { get; set; }
		public AccessRightData[] AccessRights { get; set; }
	} 
}
