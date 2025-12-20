import { Navigate } from "react-router-dom";
import { getToken } from "../utils/storage";

const PublicRoute = ({ children }) => {
  if (getToken()) {
    return <Navigate to="/rooms" replace />;
  }

  return children;
};

export default PublicRoute;