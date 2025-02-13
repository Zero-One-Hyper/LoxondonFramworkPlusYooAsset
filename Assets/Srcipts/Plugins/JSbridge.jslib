mergeInto(LibraryManager.library, {
  EmitWeb: function (id,type,json) {
  idP=UTF8ToString(id);
  typeP=UTF8ToString(type);
  jsonP=UTF8ToString(json);
  if(!window.jsBridge)return;
  window.jsBridge.UnityEmit(idP,typeP,jsonP);
  },
});