from abc import ABC, abstractmethod

class VectorStore(ABC):

    @abstractmethod
    def add_vector(self,point_id,vector,payload=None):
        pass

    @abstractmethod
    def search(self,vector,limit=5):
        pass

    @abstractmethod
    def create_payload(self, person_name):
        pass