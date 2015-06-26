using System;

namespace MiraiEngine
{
	public interface IApplicationState
	{
		Application App { get; set; }
        void OnMainLoop();
		void OnEnable();
        void OnDisable();
	}
}