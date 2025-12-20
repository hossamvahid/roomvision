import { Navigate } from "react-router-dom";
import { getToken } from "../utils/storage";

const ProtectedRoute = ({ children }) => {
  if (!getToken()) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

export default ProtectedRoute;